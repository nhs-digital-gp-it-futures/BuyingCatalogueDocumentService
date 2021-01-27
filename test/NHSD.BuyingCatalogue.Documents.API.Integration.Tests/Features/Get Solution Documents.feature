Feature:  Display uploaded file names
    As the BuyingCatalogueService
    I want to make GET documents requests for a solution
    So that I can see all available documents for a solution

Scenario Outline: Correct file names are returned for a solution
    Given There are files in the blob storage
        | SolutionId   | FileNames   |
        | <SolutionId> | <FileNames> |
    When a GET documents request is made for solution <SolutionId>
    Then a response with status code 200 is returned
    And the returned response contains the following file names
        | FileNames   |
        | <FileNames> |

    Examples:
        | SolutionId | FileNames                                                               |
        | sol1       | File1.txt, File2.txt                                                    |
        | sol2       | File1.txt, File.ouroboros                                               |
        | sol3       | File.ouroboros                                                          |
        | sol4       | File                                                                    |
        | sol5       | File with whitespace in its name.txt                                    |
        | sol6       | děkuji.pdf,Благодаря ти.blin,ありがとうございました.txt,תודה.jpeg, شكرا لكم.png, 谢 |
        | sol99      |                                                                         |
