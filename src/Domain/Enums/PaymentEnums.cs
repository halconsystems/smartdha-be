public enum AvailabilityAction { Available = 1, Blocked = 2, Booked = 3 }

public enum PaymentStatus { Unpaid = 1, Paid = 2, Refunded = 3, Failed = 4, Pending =5, PartiallyRefunded =6, Cancelled =7, Initiated = 8, Success=9 }

public enum PaymentMethod { Cash = 1, Card = 2, Wallet = 3, BankTransfer = 4, Online = 5 }

public enum PaymentProvider { None = 0, Stripe = 1, PayPal = 2, JazzCash = 3, EasyPaisa = 4, Onebill = 5 }
