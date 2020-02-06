Feature:  Display uploaded file names
    As BuyingCatalogueService
    I want make GET documents requests for a solution
    So that see which files does that soution have

Background:
	Given There are files in the blob storage
		| SolutionId | Name                             |
		| sol1       | File1.txt                        |
		| sol1       | File2.txt                        |
		| sol2       | File1.txt                        |
		| sol2       | File.ouroboros                   |
		| sol3       | File.ouroboros                   |
		| sol4       | File                             |
		| sol5       | File with whitespace in its name |
		| sol6       | děkuji.pdf                       |
		| sol6       | Благодаря ти.blin                |
		| sol6       | ありがとうございました.txt         |
		| sol6       | תודה.jpeg                        |
		| sol6       | شكرا لكم.png                     |
		| sol6       | 谢                                |

Scenario Outline: 1. Correct file names are returned for a solution
	When a GET documents request is made for solution <SolutionId>
	Then a response with status code 200 is returned
	And the returned response contains the following file names
		| FileNames   |
		| <FileNames> |

	Examples:
		| SolutionId | FileNames                                                                          |
		| sol1       | File1.txt, File2.txt                                                               |
		| sol2       | File1.txt, File.ouroboros                                                          |
		| sol3       | File.ouroboros                                                                     |
		| sol4       | File                                                                               |
		| sol5       | File with whitespace in its name                                                   |
		| sol6       | děkuji.pdf,Благодаря ти.blin,ありがとうございました.txt,תודה.jpeg, شكرا لكم.png, 谢 |
		| sol99      |                                                                                    |
