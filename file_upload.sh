az storage container create -n "container-1" --public-access blob
az storage blob upload-batch -d container-1 -s /data