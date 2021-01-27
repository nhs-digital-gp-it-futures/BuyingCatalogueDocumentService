﻿Feature:  Display file names when the storage service is not available
    As the BuyingCatalogueService
    I want to make GET documents requests for a solution
    So that I can see all available documents for a solution

Background:
    Given There are files in the blob storage
        | SolutionId | FileNames |
        | sol1       | File1.txt |

Scenario: Azure blob storage is not available
    Given the blob storage service is down
    When a GET documents request is made for solution sol1
    Then a response with status code 500 is returned
