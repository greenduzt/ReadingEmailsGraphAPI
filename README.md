# ReadingEmailsGraphAPI

ReadingEmailsGraphAPI is a .NET Core application for processing email messages retrieved from a mail server using the Microsoft Graph API.

## Services

### 1. GraphApiService

Responsible for fetching emails from the Microsoft Graph API and marking them as read.

- `GetUnreadMessagesAsync(userEmail)`: Retrieves unread email messages for a specified user.
- `MakeEmailRead(message)`: Marks an email message as read.

### 2. EmailProcessingService

Responsible for processing email messages and extracting relevant information.

- `ConvertHtmlToPlainText(html)`: Converts HTML content of an email to plain text.
- `ProcessMessage(message)`: Processes an email message, extracts sender information, subject, and message content, and handles attachments.

## Repositories

### 1. EmailRepository

Responsible for adding processed email messages to the database.

- `AddEmail(email)`: Adds an email message to the database.

### 2. EmailRepositoryFactory

Factory class for creating instances of `EmailRepository`.

- `CreateEmailRepository(config)`: Creates an instance of `EmailRepository` with the provided configuration.

## Usage

1. **Configuration**: Ensure that the Azure and Microsoft Graph API credentials are configured properly in the application configuration.

2. **Initialization**: Create instances of `GraphApiService`, `EmailProcessingService`, and `EmailRepositoryFactory` by passing the required configuration.

3. **Fetch and Process Emails**: Use `GraphApiService` to fetch unread email messages, process them using `EmailProcessingService`, and add them to the database using `EmailRepository`.

4. **Mark Emails as Read**: Use `GraphApiService` to mark email messages as read after processing.

## Prerequisites

- .NET Core SDK installed on your machine.
- Azure credentials (client ID, client secret, tenant ID) configured in the application.
- Microsoft Graph API access granted for the application.


Chamara Walaliyadde  