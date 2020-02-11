Feature:  Download existing files
    As the BuyingCatalogueService
    I want to make GET document download requests for a solution's file
    So that I make sure the contents of that file are correct

Background:
	Given There are files in the blob storage
		| SolutionId | FileNames |
		| sol1       | File1.txt |

Scenario: 1. Correct file contents are returned
	When a GET File1.txt document request is made for solution sol1
	Then a response with status code 200 is returned
	And the content of the response is equal to File1.txt belonging to sol1

Scenario: 2. No file contents are returned for a solution that doesn't exist.
	When a GET File1.txt document request is made for solution sol99
	Then a response with status code 404 is returned

Scenario: 3. No file contents are returned for a file that doesn't exist.
	When a GET File2.txt document request is made for solution sol1
	Then a response with status code 404 is returned