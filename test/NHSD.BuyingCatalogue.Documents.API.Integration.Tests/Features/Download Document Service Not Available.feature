Feature:  Download an existing file with no solution ID when the storage service is not available
    As the BuyingCatalogueService
    I want to make GET document download requests
    So that I make sure the contents of that file are correct

Background:
    Given There are files in the blob storage with no solution ID
        | FileNames |
        | File1.xlsx |

Scenario: Azure blob storage is not available
    Given the blob storage service is down
    When a GET File1.xlsx document request is made
    Then a response with status code 500 is returned
