#!/bin/bash

# =============================================================================
# Degradation Models Training Script
# Trains all 27 ML.NET regression models for degradation prediction
# =============================================================================

set -e

# Configuration
TRAIN_TIME=600  # seconds per model
MAX_PARALLEL=4  # number of parallel training jobs
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Paths
DATA_DIR="$PROJECT_ROOT/DATA/treated/ML training data/DegredationTrainingData"
OUTPUT_DIR="$PROJECT_ROOT/BACKEND_CNC-IIoT-Predictive-Maintenance-Tool-.ML/Resources/Degradation"
TEMP_DIR="$PROJECT_ROOT/temp_training"
LOG_DIR="$PROJECT_ROOT/logs"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Track results
declare -a SUCCESSFUL_MODELS=()
declare -a FAILED_MODELS=()

# Create directories
mkdir -p "$OUTPUT_DIR"
mkdir -p "$TEMP_DIR"
mkdir -p "$LOG_DIR"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  Degradation Models Training Script   ${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""
echo -e "Training time per model: ${YELLOW}${TRAIN_TIME}s${NC}"
echo -e "Max parallel jobs: ${YELLOW}${MAX_PARALLEL}${NC}"
echo -e "Data directory: ${YELLOW}${DATA_DIR}${NC}"
echo -e "Output directory: ${YELLOW}${OUTPUT_DIR}${NC}"
echo ""

# Get all CSV files
CSV_FILES=("$DATA_DIR"/*.csv)
TOTAL_MODELS=${#CSV_FILES[@]}
echo -e "Found ${GREEN}${TOTAL_MODELS}${NC} CSV files to train"
echo ""

# Function to train a single model
train_model() {
    local csv_path="$1"
    local csv_name=$(basename "$csv_path" .csv)
    local model_temp_dir="$TEMP_DIR/$csv_name"
    local log_file="$LOG_DIR/${csv_name}.log"

    echo -e "[$(date +%H:%M:%S)] ${BLUE}Starting${NC} training for: ${YELLOW}${csv_name}${NC}"

    # Create temp directory for this model
    mkdir -p "$model_temp_dir"

    # Run mlnet training
    if mlnet regression \
        --dataset "$csv_path" \
        --label-col "Label" \
        --ignore-cols "Timestamp" "Filename" \
        --has-header true \
        --train-time "$TRAIN_TIME" \
        --output "$model_temp_dir" \
        --name "$csv_name" \
        --verbosity q \
        > "$log_file" 2>&1; then

        # Find the .mlnet file (it might be in a subdirectory)
        local mlnet_file=$(find "$model_temp_dir" -name "*.mlnet" -type f | head -1)

        if [ -n "$mlnet_file" ] && [ -f "$mlnet_file" ]; then
            # Copy the trained model to the output directory
            cp "$mlnet_file" "$OUTPUT_DIR/${csv_name}.mlnet"
            echo -e "[$(date +%H:%M:%S)] ${GREEN}SUCCESS${NC}: ${csv_name}"
            return 0
        else
            echo -e "[$(date +%H:%M:%S)] ${RED}FAILED${NC}: ${csv_name} - No .mlnet file generated"
            return 1
        fi
    else
        echo -e "[$(date +%H:%M:%S)] ${RED}FAILED${NC}: ${csv_name} - Training error (see ${log_file})"
        return 1
    fi
}

# Export function and variables for parallel execution
export -f train_model
export TEMP_DIR OUTPUT_DIR LOG_DIR TRAIN_TIME RED GREEN YELLOW BLUE NC

# Start time
START_TIME=$(date +%s)

echo -e "${BLUE}Starting parallel training...${NC}"
echo ""

# Use xargs for parallel execution (more portable than GNU parallel)
printf '%s\n' "${CSV_FILES[@]}" | xargs -P "$MAX_PARALLEL" -I {} bash -c 'train_model "$@"' _ {}

# Calculate results
END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))
DURATION_MIN=$((DURATION / 60))
DURATION_SEC=$((DURATION % 60))

echo ""
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  Training Complete                    ${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Count successful models
SUCCESSFUL_COUNT=$(ls -1 "$OUTPUT_DIR"/*.mlnet 2>/dev/null | wc -l | tr -d ' ')

echo -e "Total time: ${YELLOW}${DURATION_MIN}m ${DURATION_SEC}s${NC}"
echo -e "Models trained: ${GREEN}${SUCCESSFUL_COUNT}${NC}/${TOTAL_MODELS}"
echo ""

if [ "$SUCCESSFUL_COUNT" -eq "$TOTAL_MODELS" ]; then
    echo -e "${GREEN}All models trained successfully!${NC}"
else
    echo -e "${YELLOW}Some models may have failed. Check logs in: ${LOG_DIR}${NC}"
fi

# List generated models
echo ""
echo -e "${BLUE}Generated model files:${NC}"
ls -lh "$OUTPUT_DIR"/*.mlnet 2>/dev/null || echo "No model files found"

# Cleanup temp directory
echo ""
read -p "Clean up temporary training files? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    rm -rf "$TEMP_DIR"
    echo -e "${GREEN}Temporary files cleaned up.${NC}"
fi

echo ""
echo -e "${GREEN}Done!${NC}"
