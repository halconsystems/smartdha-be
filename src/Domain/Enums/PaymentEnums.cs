public enum AvailabilityAction { Available = 1, Blocked = 2 }

public enum BookingStatus { Provisional = 1, Confirmed = 2, CheckedIn = 3, CheckedOut = 4, Cancelled = 5 }

public enum PaymentStatus { Authorized = 1, Paid = 2, Refunded = 3, Failed = 4, Pending =5, PartiallyRefunded =6, Cancelled =7, Captured = Paid }

public enum PaymentMethod { Cash = 1, Card = 2, Wallet = 3, BankTransfer = 4 }

public enum PaymentProvider { None = 0, Stripe = 1, PayPal = 2, JazzCash = 3, EasyPaisa = 4, Onebill = 5 }
