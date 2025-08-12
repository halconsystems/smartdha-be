public enum AvailabilityAction { Available = 1, Blocked = 2 }

public enum ReservationStatus { Draft = 1, AwaitingPayment, Converted, Expired, Cancelled }

public enum BookingStatus { Provisional = 1, Confirmed, CheckedIn, CheckedOut, Cancelled }

public enum PaymentIntentStatus { RequiresPayment = 1, Processing, Succeeded, Cancelled, Expired }

public enum PaymentStatus { Authorized = 1, Paid, Refunded, Failed }

public enum PaymentMethod { Cash = 1, Card, Wallet, BankTransfer }

public enum PaymentProvider { None = 0, Stripe, PayPal, JazzCash, EasyPaisa }
