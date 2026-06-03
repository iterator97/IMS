# IMS: inventory-managment-system

# IMS.Integration.Tests - Prerequisites
# To run the integration tests, you need to have the following installed on your machine:
# - .NET 10.0 SDK
# - Docker Desktop 
# 
# You need to setup the following environment variable in IMS.Integration.Tests/.env file, sample below:
# Host=localhost;Port=5433;Database=ims_integration_tests;Username=user;Password=password
# To run the tests, navigate to the IMS.Integration.Tests directory and execute the following command:
# docker compose up -d (integration database setup)
# dotnet test
