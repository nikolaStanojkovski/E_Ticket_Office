# Online Ticket Office Application
-----------------------------------------------------------------------------------
## Integrated systems project by Nikola Stanojkovski
-----------------------------------------------------------------------------------

Online shop application for ordering and managing cinema tickets developed with the help of <b>.NET Core</b> framework on back-end, and <b>Server Side Razor</b> on front-end.
<br/> <br/>
The application uses <b>SQL Server</b> as a database server, <b>Onion</b> architecture (<b>Domain, Repository, Service, Web</b> layers) as a main architectural pattern, and, <b>C#</b> as a main programming language. <br/> <br/>
The application has the following functionalities:
<br/>
- User Registration / Login with full authorization mechanism throughout the whole application.
- User Management (Adding users to roles)
- <i>CRUD</i> Operations for tickets
- Review of all available tickets
- Filtering the tickets by valid date
- Full shopping cart functionality for all users
- Full order management functionality for all users
- <i>Email</i> notification with the help of the <b><i>SMTP</i></b> protocol
- Ordering tickets with the help of <b><i>Stripe</i></b> payment provider
- <i>PDF</i> Invoice export for orders for all users with the help of <b><i>Gembox.Document</i></b> library
- <i>Excel</i> Inovice export for tickets, filtered by genre with the help of <b><i>ClosedXML</i></b> library
- Import of users from an <i>Excel</i> file by username, password and user role with the help of <b><i>ExcelDataReader</i></b> library
