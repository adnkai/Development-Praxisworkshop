echo "Creating release annotation in Application Insights"

APPINSIGHTS_ID=$(az resource show -g $1-rg -n $1-applicationinsights --resource-type "microsoft.insights/components" --query id -o tsv)
UUID=$(cat /proc/sys/kernel/random/uuid)
releaseName=$2
releaseDescription=$3
triggerBy=$4
eventTime=`date '+%Y-%m-%dT%H:%M:%S' -u`
category="Deployment"

data='{ "Id": "'$UUID'", "AnnotationName": "'$releaseName'", "EventTime":"'$eventTime'", "Category":"'$category'", "Properties":"{ \"ReleaseName\":\"'$releaseName'\", \"ReleaseDescription\" : \"'$releaseDescription'\", \"TriggerBy\": \"'$triggerBy'\" }"}'

az rest --method put --uri "$APPINSIGHTS_ID/Annotations?api-version=2015-05-01" --body "$data" -o none