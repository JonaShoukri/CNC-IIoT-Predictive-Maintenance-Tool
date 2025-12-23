#!/bin/bash

# =============================================================================
# Degradation Models Training Script (Background-friendly version)
# =============================================================================

TRAIN_TIME=600
MAX_PARALLEL=4
PROJECT_ROOT="/Users/jonas/Documents/GitHub/CNC-IIoT-Predictive-Maintenance-Tool/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-"
DATA_DIR="$PROJECT_ROOT/DATA/treated/ML training data/DegredationTrainingData"
OUTPUT_DIR="$PROJECT_ROOT/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-.ML/Resources/Degradation"
TEMP_DIR="$PROJECT_ROOT/temp_training"
LOG_DIR="$PROJECT_ROOT/logs"

mkdir -p "$OUTPUT_DIR" "$TEMP_DIR" "$LOG_DIR"

echo "=========================================="
echo "  Starting Degradation Models Training"
echo "=========================================="
echo "Start time: $(date)"
echo ""

# List of all models to train
MODELS=(
    "RMS"
    "Peak"
    "CrestFactor"
    "Kurtosis"
    "Skewness"
    "StdDev"
    "PeakToPeak"
    "Mean"
    "Variance"
    "DominantFreq_Hz"
    "DominantFreq_Mag"
    "Energy_0_500Hz"
    "Energy_500_1000Hz"
    "Energy_1000_2000Hz"
    "Energy_2000_4000Hz"
    "Energy_4000_6000Hz"
    "Energy_6000_8000Hz"
    "Energy_8000_10240Hz"
    "Energy_BPFO"
    "Energy_BPFI"
    "Energy_BSF"
    "Energy_FTF"
    "Energy_BPFO_2x"
    "Energy_BPFI_2x"
    "Energy_BPFO_3x"
    "Energy_BPFI_3x"
    "Revolutions"
)

train_model() {
    local model_name="$1"
    local csv_path="$DATA_DIR/${model_name}.csv"
    local model_temp="$TEMP_DIR/${model_name}"
    local log_file="$LOG_DIR/${model_name}.log"

    echo "[$(date +%H:%M:%S)] Starting: $model_name"

    mkdir -p "$model_temp"

    if mlnet regression \
        --dataset "$csv_path" \
        --label-col "Label" \
        --ignore-cols "Timestamp" "Filename" \
        --has-header true \
        --train-time "$TRAIN_TIME" \
        --output "$model_temp" \
        --name "$model_name" \
        --verbosity q \
        > "$log_file" 2>&1; then

        # Find and copy the .mlnet file
        mlnet_file=$(find "$model_temp" -name "*.mlnet" -type f 2>/dev/null | head -1)

        if [ -n "$mlnet_file" ] && [ -f "$mlnet_file" ]; then
            cp "$mlnet_file" "$OUTPUT_DIR/${model_name}.mlnet"
            echo "[$(date +%H:%M:%S)] SUCCESS: $model_name"
            return 0
        else
            echo "[$(date +%H:%M:%S)] FAILED: $model_name (no .mlnet file)"
            return 1
        fi
    else
        echo "[$(date +%H:%M:%S)] FAILED: $model_name (training error - see $log_file)"
        return 1
    fi
}

# Track running jobs
running_jobs=()
completed=0
failed=0
total=${#MODELS[@]}

echo "Training $total models with $MAX_PARALLEL parallel jobs..."
echo ""

for model in "${MODELS[@]}"; do
    # Wait if we have too many jobs
    while [ ${#running_jobs[@]} -ge $MAX_PARALLEL ]; do
        new_running=()
        for job in "${running_jobs[@]}"; do
            if kill -0 "$job" 2>/dev/null; then
                new_running+=("$job")
            else
                wait "$job" && ((completed++)) || ((failed++))
            fi
        done
        running_jobs=("${new_running[@]}")
        [ ${#running_jobs[@]} -ge $MAX_PARALLEL ] && sleep 5
    done

    # Start new job
    train_model "$model" &
    running_jobs+=($!)
done

# Wait for remaining jobs
for job in "${running_jobs[@]}"; do
    wait "$job" && ((completed++)) || ((failed++))
done

echo ""
echo "=========================================="
echo "  Training Complete"
echo "=========================================="
echo "End time: $(date)"
echo "Successful: $completed"
echo "Failed: $failed"
echo ""
echo "Models in output directory:"
ls -lh "$OUTPUT_DIR"/*.mlnet 2>/dev/null || echo "No models found"
echo ""

# Cleanup temp
rm -rf "$TEMP_DIR"
echo "Temporary files cleaned up."
echo ""
echo "Done!"
