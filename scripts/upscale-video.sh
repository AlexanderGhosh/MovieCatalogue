#!/bin/bash

while [[ $# -gt 0 ]]; do
    case $1 in
        -v)
            VERBOSE="true"
            shift
            ;;
        -i|--input)
            INPUT="$2"
            shift 2
            ;;
        -o|--output)
            OUTPUT="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

if [[ -z "$INPUT" ]]; then
    echo "Error: -i or --input is required"
    exit 1
fi

if [[ -z "$OUTPUT" ]]; then
    echo "Error: -o or --output is required"
    exit 1
fi

ffmpeg -i "$INPUT" -vf scale=iw*2:ih*2:flags=neighbor -c:v libx264 -preset slow -crf 18 "$OUTPUT"

# upscale-video.sh -i "/var/www/MovieCatalogue/Movies/80886D21-C041-4363-8746-F39FAF6A1EBD.blob" -o "~/stwk.mp4"