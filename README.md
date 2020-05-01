# [DEPRECATED]
This repository was only there to build the first version of the .net generator. The sample application is here. 
https://github.com/jhipster/jhipster-sample-app-dotnetcore

# JHipsterNetSampleApplication

[![Build Status](https://travis-ci.org/jhipster/jhipster-net-sample-app-template.svg?branch=master)](https://travis-ci.org/jhipster/jhipster-net-sample-app-template)

*This is a WIP project, please don't use it as is.*

This application used as template to build the [jhipster-dotnetcore](https://github.com/jhipster/jhipster-dotnetcore) generator. The objective is port JHipster to the .Net core platform thus be able to generate a `.Net Core` monolith application.

# Structure

## JHipsterNet
This project acts as a library. It contains transverse concerns such as Pagination, Config management, etc.
In the future, this project will be moved to a dedicate repository and packaged as "classic" nuget dependency.

The development rules are:
1. Avoid to write code, you should rely on the .Net Core ecosystem as much as possible.
2. Do not try to reimplement Spring in .Net. Spring is a huge and complete platform if there is missing parts on the .Net world, use Spring as an inspiration, isolate the missing missing and be pragmatic.   

## JHipsterNetSampleApplication
The app is the `C#/.Net Core` translation of the [JHipster Sample App](https://github.com/jhipster/jhipster-sample-app). 

The development rules are:
1. Stay aligned with [JHipster policies](https://www.jhipster.tech/policies/) (use default configuration, use explicit versions, etc.)
2. Follow C#/.Net Core conventions (casing, namespaces names, DI, etc.)
3. Be explicit and do not use dark magic (weird reflection, assembly scanning etc). 

## JHipsterNetSampleApplication.Test
The test project is the `C#/.Net Core` translation of the [JHipster Sample App](https://github.com/jhipster/jhipster-sample-app) backend end to end test.

The development rules are:
1. The priority is to cover 100% of the Java tests (usually mapping 1:1).
2. You can remove some tests if implies a standard usage of the C# language.
3. You can add test if you re-code a feature normally covered by Spring.
 

# Test

To run the solution tests, simply run: `dotnet test`

# Running the app in a Docker container

1. Build the Docker image of the app
```bash
docker build -f "[Dockerfile path]" -t [An image name]:[A tag] "[Application root path]"
```
2. Run your image in a Docker container
```bash
docker run -d -p [A host port]:80 [Image name]:[Image tag]
```
3. Open your favorite browser at ```localhost:[Chosen host port]``` and enjoy ! :whale:
