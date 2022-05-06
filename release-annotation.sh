echo "Creating release annotation in Application Insights"

APPINSIGHTS_ID=$( az resource show -g $4 -n $5 --resource-type "microsoft.insights/components" --query id -o tsv)
UUID=$(uuidgen)
releaseName=$1
releaseDescription=$2
triggerBy=$3
eventTime=`date '+%Y-%m-%dT%H:%M:%S' -u`
category="Deployment"

data='{ "Id": "'$UUID'", "AnnotationName": "'$releaseName'", "EventTime":"'$eventTime'", "Category":"'$category'", "Properties":"{ \"ReleaseName\":\"'$releaseName'\", \"ReleaseDescription\" : \"'$releaseDescription'\", \"TriggerBy\": \"'$triggerBy'\" }"}'

az rest --method put --uri "$APPINSIGHTS_ID/Annotations?api-version=2015-05-01" --body "$data" -o none
