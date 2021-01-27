Feature: Download existing files without a solution ID
    As the BuyingCatalogueService
    I want to make GET document download requests
    So that I make sure the contents of that file are correct

Background:
    Given There are files in the blob storage with no solution ID
        | FileNames  |
        | File1.xlsx |

Scenario: Correct file contents are returned
    When a GET File1.xlsx document request is made
    Then a response with status code 200 is returned
    Then the content of the response is equal to 'File1.xlsx' belonging to NULL

Scenario: No file contents are returned for a file that doesn't exist.
    When a GET File2.xlsx document request is made
    Then a response with status code 404 is returned
