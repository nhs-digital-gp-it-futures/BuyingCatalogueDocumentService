FROM mcr.microsoft.com/azure-cli 
WORKDIR /data
COPY data ./
COPY file_upload.sh /
ENTRYPOINT ["/bin/bash", "/file_upload.sh"]