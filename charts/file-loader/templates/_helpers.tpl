{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "file-loader.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "file-loader.fullname" -}}
{{- $name := include "file-loader.chart" . -}}

{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-"  -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "file-loader.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | trunc 63 | replace "+" "-" | replace "." "-" | lower | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "file-loader.labels" -}}
helm.sh/chart: {{ include "file-loader.chart" . }}
{{ include "file-loader.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "file-loader.selectorLabels" -}}
app.kubernetes.io/name: {{ include "file-loader.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "file-loader.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "file-loader.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}

{{/*
Defines which image:tag and what pull policy to use
*/}}
{{- define "file-loader.image.properties" -}}
{{- $localImageName := .Values.image.repository | replace "gpitfuturesdevacr.azurecr.io/" "" -}}
{{- $imageName := ternary $localImageName (printf "%s:%s" .Values.image.repository .Chart.AppVersion) .Values.useLocalImage -}}
image: {{ $imageName | quote }}
imagePullPolicy: {{ "IfNotPresent" | quote }}
{{- end }}
