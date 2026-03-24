using System;
using System.Net.Http.Headers;
using BankApp.Models.Enums;

namespace BankApp.Client.Extensions;

public static class NotificationTypeExtensions
{
    public static string ToDisplayName(this NotificationType type) => type switch
    {
        NotificationType.InboundTransfer => "Inbound Transfer",
        NotificationType.OutboundTransfer => "Outbound Transfer",
        NotificationType.LowBalance => "Low Balance",
        NotificationType.DuePayment => "Due Payment",
        NotificationType.SuspiciousActivity => "Suspicious Activity",
        _ => type.ToString()
    };

    public static NotificationType FromString(string value) => value switch
    {
        "Payment" => NotificationType.Payment,
        "InboundTransfer" => NotificationType.InboundTransfer,
        "OutboundTransfer" => NotificationType.OutboundTransfer,
        "LowBalance" => NotificationType.LowBalance,
        "DuePayment" => NotificationType.DuePayment,
        "SuspiciousActivity" => NotificationType.SuspiciousActivity,
        _ => throw new ArgumentException($"Unknown NotificationType: {value}")
    };
}