# &#x2654; MockSrv

Postman, Ready Api et d'autres offrent la possibilté de créer des mocks d'Api. Cependant, Ces Mock Serveurs sont deployés sur le Cloud.

Pour ceux qui ne veulent pas utiliser le cloud pour une raison ou une autre, ce repo offre une alternative gratuite.

Travaux Restants et/ou en cours :
- Request Headers
    - &check; none
    - &check; custom
- Request Body
    - &check; none
    - &#x2610; form-data
    - &#x2610; x-www-form-urlencoded
    - &#x2610; binary
    - &#x2610; GraphQL
    - &check; Raw
        - &check; Text
        - &#x2612; Javascript
        - &check; Json
        - &check; Html
        - &check; Xml


![](captureWeb.png)
![](captureDB.png)

# Documentation to learn
## Radzen
https://blazor.radzen.com/get-started
## Call modal
https://blazor.radzen.com/datagrid-column-template?theme=material3
## Identity
https://medium.com/@mohamed.ebrahim.mohsen/net8-identity-register-login-email-confirmation-and-two-factor-authentication-2fa-c8acfbc3e14c#id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxNGZiOWIwODcxODBiYzAzMDMyODQ1MDBjNWY1NDBjNmQ0ZjVlMmYiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIyMTYyOTYwMzU4MzQtazFrNnFlMDYwczJ0cDJhMmphbTRsamRjbXMwMHN0dGcuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIyMTYyOTYwMzU4MzQtazFrNnFlMDYwczJ0cDJhMmphbTRsamRjbXMwMHN0dGcuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDI0NjE2ODY3MDU3NzU0NzIxODAiLCJlbWFpbCI6InNhZHJpLmZlcnRhbmlAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5iZiI6MTc0MTUzMzM5OCwibmFtZSI6IlNhZHJpIEZlcnRhbmkiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jSmtuYVlqUzQ4cFhOR3R4eVJBdlJXcHoyTE44UEpVZnNuQlFyOF9pWGppeHM0Qi03MW09czk2LWMiLCJnaXZlbl9uYW1lIjoiU2FkcmkiLCJmYW1pbHlfbmFtZSI6IkZlcnRhbmkiLCJpYXQiOjE3NDE1MzM2OTgsImV4cCI6MTc0MTUzNzI5OCwianRpIjoiZDAxNTAyNWYwY2RkNTQ1Zjc4ZjIwMzk4YjQzMzg1MWFmMDgxYzg0ZCJ9.BSz-kZYEOhkEvSf2jKET0YfB93yaqR7Trm20BIC7GcrInDcFlFpgQTuosIgSCFPjY7dRNpdvBlRacl0VZPBbT_mnlLCFj7p5iMdtyqdJI09kHI6LWKYDN5X-5bXQ8bQwh8IhZsDyoEQslcJOlYwYWcL4pDZg8KmwaOAgbDw0FoONjlflBdp3nUOYIs91G3mF_i6cGhgkDJcsHt4pR5m9DrK1bSyqrzfKSXbn-wmXlZvKCHEQZf-ygcdq1u2PfBliDYJmbcWh6uZ0kVEfd7WhmM3FaEDf9a1qANgA5lwKJRV94DRB3dUtX_l-Q-npmBhcon9So31xTXTzI52ijvZPPQ
https://stackoverflow.com/questions/74856694/i-need-a-simple-example-of-a-login-page-in-blazor-server-app-without-to-see-or-a
https://github.com/dotnet/aspnetcore/blob/main/src/Identity/samples/IdentitySample.DefaultUI/Areas/Identity/Pages/Account/Register.cshtml.cs

## Add new user
```csharp
await UserManager.AddLoginAsync(new IdentityUser { Email = "sadri.fertani@gmail.com" }, new UserLoginInfo("password", "Qwerty123.", "password"));
```

https://github.com/radzenhq/radzen-examples/blob/master/CRMDemoBlazor/server/Controllers/AccountController.cs