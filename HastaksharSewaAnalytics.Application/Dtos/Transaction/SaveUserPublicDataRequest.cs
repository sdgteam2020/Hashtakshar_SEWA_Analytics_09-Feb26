namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveUserPublicDataRequest(
   string Public_Key,
   string SerialNo,
   bool TokenValid,
   string ValidFrom,
   string ValidTo
    );
