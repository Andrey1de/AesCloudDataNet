# heroku addons:create heroku-postgresql:hobby-dev -a  aes-cloud-data

# C:\_nadia\new>heroku pg:info
=== DATABASE_URL
Plan:                  Hobby-dev
Status:                Available
Connections:           0/20
PG Version:            13.2
Created:               2021-04-14 09:25 UTC
Data Size:             7.9 MB
Tables:                0
Rows:                  0/10000 (In compliance)
Fork/Follow:           Unsupported
Rollback:              Unsupported
Continuous Protection: Off
Add-on:                postgresql-closed-13276

# heroku pg:credentials:url DATABASE

heroku pg:credentials:url DATABASE  -a  aes-cloud-data
Connection information for default credential.
Connection info string:
   "dbname=d28frsq3e3m2c7 host=ec2-3-233-43-103.compute-1.amazonaws.com port=5432 user=qzauftxolisgbf password=6d7750165ba9b416a40b348dd6e6a9a734427c2a7464f0ce9828872c9975ff9e sslmode=require"
Connection URL:
   postgres://qzauftxolisgbf:6d7750165ba9b416a40b348dd6e6a9a734427c2a7464f0ce9828872c9975ff9e@ec2-3-233-43-103.compute-1.amazonaws.com:5432/d28frsq3e3m2c7
   
   
  Database Credentials
Get credentials for manual connections to this database.

Cancel
Please note that these credentials are not permanent.

Heroku rotates credentials periodically and updates applications where this database is attached.

Host : ec2-3-233-43-103.compute-1.amazonaws.com
Database : d28frsq3e3m2c7
User : qzauftxolisgbf
Port : 5432
Password : 6d7750165ba9b416a40b348dd6e6a9a734427c2a7464f0ce9828872c9975ff9e
URI : postgres://qzauftxolisgbf:6d7750165ba9b416a40b348dd6e6a9a734427c2a7464f0ce9828872c9975ff9e@ec2-3-233-43-103.compute-1.amazonaws.com:5432/d28



# Using an Existing Database (Database-First)
The Npgsql EF Core provider also supports reverse-engineering a code model from an existing PostgreSQL database ("database-first"). 
To do so, use dotnet CLI to execute the following:
Using an Existing Database (Database-First)
## dotnet ef dbcontext scaffold "Host=my_host;Database=my_db;Username=my_user;Password=my_pw" Npgsql.EntityFrameworkCore.PostgreSQL

## dotnet ef dbcontext scaffold "host=localhost;port=5432;database=postgres;userid=postgres;password=1q1q" Npgsql.EntityFrameworkCore.PostgreSQL

  PM>dotnet ef dbcontext scaffold "host=localhost;port=5432;database=clouddata;userid=postgres;password=1q1q" Npgsql.EntityFrameworkCore.PostgreSQL
