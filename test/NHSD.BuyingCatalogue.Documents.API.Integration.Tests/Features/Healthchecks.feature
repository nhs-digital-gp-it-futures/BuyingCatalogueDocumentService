Feature: Document Service healthchecks
    As a document service
    I want to be able to check the health of my dependencies
    So that I can behave accordingly

@5152
Scenario: Blob storage is up
    Given the Bob Storage is up
    When the dependency health-check endpoint is hit
    Then the response will be Healthy

@5152
Scenario: Blob storage is down
    Given the Bob Storage is down
    When the dependency health-check endpoint is hit
    Then the response will be Unhealthy
