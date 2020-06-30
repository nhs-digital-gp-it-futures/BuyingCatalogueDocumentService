#!/bin/bash
CONTAINER_NAME=${CONTAINER_NAME:-container-1}
INSERT_TEST_DATA=${INSERT_TEST_DATA:-true}

az storage container create -n $CONTAINER_NAME --public-access blob

if [ $INSERT_TEST_DATA = "true" ]; then
    az storage blob upload-batch -d $CONTAINER_NAME -s /data
    az storage blob update --container-name $CONTAINER_NAME --name "non-solution/compare-solutions.xlsx" --content-type "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
fi