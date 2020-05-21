from flask import Flask, request
from flask_restful import Resource, Api
from flask_jsonpify import jsonify
import boto3
import time
import sys
import json

app = Flask(__name__)
api = Api(app)

class Textract(Resource):
	def get(self, fileName):

		analysis = runAnalysis(fileName)

		return analysis[0]


def runAnalysis(fileName):

	jobId = startJob("pdfidentify", fileName)
	isJobComplete(jobId)
	return getJobResults(jobId)


def startJob(s3BucketName, objectName):
    client = boto3.client('textract')
    response = client.start_document_analysis(
        DocumentLocation={
            'S3Object': {
                'Bucket': s3BucketName,
                'Name': objectName
            }
        },
        FeatureTypes=['TABLES', 'FORMS'])

    return response["JobId"]

def isJobComplete(jobId):
    time.sleep(1)
    client = boto3.client('textract')
    response = client.get_document_analysis(JobId=jobId)
    status = response["JobStatus"]

    while(status == "IN_PROGRESS"):
        time.sleep(5)
        response = client.get_document_analysis(JobId=jobId)
        status = response["JobStatus"]

    return status

def getJobResults(jobId):

    pages = []

    time.sleep(5)

    client = boto3.client('textract')
    response = client.get_document_analysis(JobId=jobId)

    pages.append(response)
    nextToken = None
    if 'NextToken' in response:
        nextToken = response['NextToken']

    while nextToken:
        time.sleep(5)

        response = client.get_document_analysis(JobId=jobId, NextToken=nextToken)

        pages.append(response)
        nextToken = None
        if 'NextToken' in response:
            nextToken = response['NextToken']

    return pages



api.add_resource(Textract, '/<string:fileName>')

if __name__ == '__main__':
	app.run(debug=True,port='5003', ssl_context='adhoc')
