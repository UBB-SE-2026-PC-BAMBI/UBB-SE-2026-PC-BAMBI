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
}