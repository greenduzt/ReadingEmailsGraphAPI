# Microsoft Graph Email Reader and Attachment Downloader

This console application demonstrates how to read emails from a Microsoft Exchange server using the Microsoft Graph API, mark emails as read, and download attachments.

## Description

This program utilizes the Microsoft Graph SDK along with Azure Identity to authenticate and access emails from a specified user's inbox. It filters and retrieves only unread emails, displays email details such as sender name, recipient, subject, arrival time, and message content. Additionally, it downloads PDF attachments from unread emails to a specified folder.

## Features

- Authenticate with Azure Identity using client secret credentials.
- Retrieve unread emails from a specified user's inbox using the Microsoft Graph SDK.
- Display email details such as sender name, recipient, subject, arrival time, and message content.
- Download PDF attachments from unread emails to a specified folder.
- Mark emails as read after processing.

## Getting Started

### Prerequisites

- .NET Core SDK installed on your machine.
- Azure Active Directory (Azure AD) application registered with the required permissions to access Microsoft Graph.
- User secrets configured for the Azure AD application containing tenant ID, client ID, client secret, and user email.

### Configuration

1. Clone this repository to your local machine.
2. Open the project in your preferred IDE.
3. Configure user secrets by running the following commands in the terminal:

```bash
dotnet user-secrets set "GraphMail:TenantId" "<YourTenantId>"
dotnet user-secrets set "GraphMail:ClientId" "<YourClientId>"
dotnet user-secrets set "GraphMail:ClientSecret" "<YourClientSecret>"
dotnet user-secrets set "GraphMail:Email" "<UserEmail>"

## Authors

Contributors names and contact info

Chamara Walaliyadde  