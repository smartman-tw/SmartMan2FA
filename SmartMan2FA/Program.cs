using Google.Authenticator;

namespace SmartMan2FA
{
    internal class Program
    {
        // -g -a FrankHuang -k mySecretKey to generate a new setup code
        // -v -k mySecretKey -p 123456 to validate a code
        // return 0 if the code is valid or the qrcode is generated successfully, otherwise return -1
        static int Main(string[] args)
        {
            // success code = 0, error code = -1
            int successCode = 0;
            int errorCode = -1;

            // Create a logger
            var logger = new SmartManLogger("logs");
            logger.LogText("Starting SmartMan2FA.");
            try
            {
                // Check if the arguments are valid
                bool generateSetupCode = Array.IndexOf(args, "-g") != -1;
                bool validateCode = Array.IndexOf(args, "-v") != -1;
                if (!generateSetupCode && !validateCode)
                {
                    logger.LogText("Invalid arguments. Please use the format '-g -a FrankHuang -k mySecretKey' to generate a new setup code or '-v -k mySecretKey -p 123456' to validate a pin code.");
                    return errorCode;
                }
                if (generateSetupCode)
                {
                    // accept the arguments based on the format '-a FrankHuang -k mySecretKey'
                    // check if the arguemnt is valid
                    if (args.Length < 4 || Array.IndexOf(args, "-a") == -1 || Array.IndexOf(args, "-k") == -1)
                    {
                        logger.LogText("Invalid arguments. Please use the format '-a FrankHuang -k mySecretKey'.");
                        return errorCode;
                    }
                    // get the account and secret key
                    string account = args[Array.IndexOf(args, "-a") + 1];
                    string secretKey = args[Array.IndexOf(args, "-k") + 1];
                    // create a new instance of the TwoFactorAuthenticator
                    var tfa = new TwoFactorAuthenticator();
                    // generate a new setup code
                    SetupCode setupInfo = tfa.GenerateSetupCode("SmartMan2FA", account, secretKey, false);
                    logger.LogText($"Setup code and QRCode for the account {account} have been generated successfully.");
                    // write the manual entry setup code to a text file
                    string manualEntrySetupCode = setupInfo.ManualEntryKey;
                    // write the QR code setup code to a text file
                    string setupCodeFileName = "setup_code.txt";
                    using (StreamWriter file = new StreamWriter(setupCodeFileName))
                    {
                        file.WriteLine(manualEntrySetupCode);
                    }
                    logger.LogText($"Setup code has been output to {setupCodeFileName}.");
                    string qrCodeBase64String = setupInfo.QrCodeSetupImageUrl;
                    // extract the base64 QRCode from the setup code
                    qrCodeBase64String = qrCodeBase64String.Split(new string[] { "base64," }, StringSplitOptions.None)[1];
                    // convert the base64 QRCode to JPG
                    byte[] qrCodeBytes = Convert.FromBase64String(qrCodeBase64String);
                    string qrCodeFileName = "qrcode.jpg";
                    using (FileStream file = new FileStream(qrCodeFileName, FileMode.Create))
                    {
                        file.Write(qrCodeBytes, 0, qrCodeBytes.Length);
                    }
                    logger.LogText($"QRCode has been output to {qrCodeFileName}.");
                    return successCode;
                }
                else if (validateCode)
                {
                    // check if the arguemnt is valid
                    if (args.Length < 4 || Array.IndexOf(args, "-k") == -1 || Array.IndexOf(args, "-p") == -1)
                    {
                        logger.LogText("Invalid arguments. Please use the format '-k mySecretKey -p 123456'.");
                        return errorCode;
                    }
                    // get the secret key and pin code
                    string secretKey = args[Array.IndexOf(args, "-k") + 1];
                    string pinCode = args[Array.IndexOf(args, "-p") + 1];
                    // create a new instance of the TwoFactorAuthenticator
                    var tfa = new TwoFactorAuthenticator();
                    // validate the pin code
                    bool pinCodeIsValid = tfa.ValidateTwoFactorPIN(secretKey, pinCode);
                    if (pinCodeIsValid)
                    {
                        logger.LogText("The pin code is valid.");
                        return successCode;
                    }
                    else
                    {
                        logger.LogText("The pin code is invalid.");
                        return errorCode;
                    }
                }
                logger.LogText("Unexpected error.");
                return errorCode;
            }
            catch (Exception ex)
            {
                logger.LogError("An unhandled exception occurred.", ex);
                return errorCode;
            }
            finally
            {
                logger.LogText("Exiting SmartMan2FA.");
            }
        }
    }
}
