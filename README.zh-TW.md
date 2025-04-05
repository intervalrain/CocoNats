# CocoNats

[English](README.md) | 繁體中文

CocoNats 是一個輕量級框架，用於在 .NET 中構建基於 NATS 的微服務，靈感來自 ABP Framework 的應用服務模式。

## 概述

CocoNats 通過提供面向服務的抽象層簡化了 .NET 應用程序中的 NATS 消息集成。類似於 ABP 的 ApplicationService 動態生成 Controllers，CocoNats 自動生成 NATS 處理程序，使開發人員能夠專注於應用邏輯而不是消息基礎設施。

## 功能特點

- **自動處理程序生成**：專注於業務邏輯，而 CocoNats 處理消息基礎設施
- **多種通訊模式**：
  - 請求/響應
  - 發布/訂閱
  - 推送/拉取與隊列組 (JetStream)
  - 鍵值存儲
  - 對象存儲
- **簡化的服務開發**：基於聲明性屬性的配置
- **約定式註冊**：自動發現和註冊服務

## 開始使用

### 安裝

從 NuGet 安裝 CocoNats 包。

### 基本用法

1. 使用屬性創建 NATS 服務
2. 在應用程序啟動時註冊服務

## 許可證

本項目採用 MIT 許可證 - 詳見 LICENSE 文件。