Feature:  Download an existing file with no solution id when the storage service is not available
    As the BuyingCatalogueService
    I want to make GET document download requests
    So that I make sure the contents of that file are correct

Background:
	Given There are files in the blob storage with no solution id
		| FileNames |
		| File1.txt |

Scenario: 1. Azure blob storage is not available
	Given the blob storage service is down
	When a GET File1.txt document request is made
	Then a response with status code 500 is returned
