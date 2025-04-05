# CocoNats

English | [繁體中文](README.zh-TW.md)

CocoNats is a lightweight framework for building NATS-based microservices in .NET, inspired by ABP Framework's application service pattern.

## Overview

CocoNats simplifies NATS messaging integration in .NET applications by providing a service-oriented abstraction layer. Similar to how ABP's ApplicationService dynamically generates Controllers, CocoNats automatically generates NATS handlers, allowing developers to focus on application logic rather than messaging infrastructure.

## Features

- **Automatic Handler Generation**: Focus on business logic while CocoNats handles the messaging infrastructure
- **Multiple Communication Patterns**:
  - Request/Response
  - Publish/Subscribe
  - Push/Pull with Queue Groups (JetStream)
  - Key-Value Store
  - Object Store
- **Simplified Service Development**: Declarative attribute-based configuration
- **Convention-based Registration**: Automatic discovery and registration of services

## Getting Started

### Installation

Install the CocoNats packages from NuGet.

### Basic Usage

1. Create a NATS Service using attributes
2. Register services in your application's startup

## License

This project is licensed under the MIT License - see the LICENSE file for details.