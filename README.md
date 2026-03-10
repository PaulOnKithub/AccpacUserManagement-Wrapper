# Sage 300 IGA functionalities Wrapper

## Overview
This project serves as a comprehensive web API wrapper designed to enable Identity Governance and Administration (IGA) functionalities for the Sage 300 ERP system. By bridging the gap between modern, RESTful web services and the legacy Sage 300 COM APIs, this wrapper allows seamless integration with enterprise identity management platforms, automated user provisioning systems, and centralized access control solutions.

## Architecture
The application is built using ASP.NET Web API / MVC on the .NET framework. It leverages the Sage 300 Advantage SDK (COM interop) to establish sessions, perform transactions, and interact safely and efficiently with Sage 300 data dictionaries, particularly focusing on system security, user administration, and session management.

## Key Functionalities
*   **User Provisioning & Deprovisioning:** Automate the creation, modification, and deletion of Sage 300 user accounts and associated security groups.
*   **Access Review & Entitlement Management:** Query existing users, security groups, and user authorizations to audit and manage permissions effectively.
*   **Secure Session Management:** Handles the complex COM session initiation and teardown safely over an HTTP lifecycle, ensuring the underlying database connections are not orphaned.
*   **RESTful API Interface:** Exposes standard JSON-based endpoints (`/api/...`) that are easily consumable by modern IGA platforms like SailPoint, Okta, or customized internal identity solutions.

## Getting Started

### Prerequisites
*   Windows Server or development environment capable of running ASP.NET Framework applications.
*   Sage 300 ERP installed locally or accessible via the network.
*   Sage 300 Advantage SDK / COM objects registered on the host machine.
*   Valid Sage 300 credentials and database access for the API to utilize.

### Configuration
1. Open `Web.config` and configure the necessary application settings, particularly the Sage 300 database IDs, system IDs, and any required service credentials under the `<appSettings>` node.
2. Ensure the identity running the IIS Application Pool has the appropriate file system permissions to access the Sage 300 Shared Data directories.

### Running the Application
Open the solution in Visual Studio, build the project, and start the debugger (IIS Express). The application will launch a landing page detailing the API, from which you can explore the available endpoints or use tools like Postman to interact with the IGA services.
