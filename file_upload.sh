az storage container create -n "container-1" --public-access blob
az storage blob upload-batch -d container-1 -s /data
az storage blob update --container-name "container-1" --name "non-solution/compare-solutions.xlsx" --content-type "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"