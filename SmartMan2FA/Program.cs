using Google.Authenticator;

using System.Security.Cryptography;

namespace SmartMan2FA
{
    internal class Program
    {
        // -g -a FrankHuang -k mySecretKey to generate a new setup code
        // -v -k mySecretKey -p 123456 to validate a code
        // -c to create a new key
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
                bool createKey = Array.IndexOf(args, "-c") != -1;
                if (!generateSetupCode && !validateCode && !createKey)
                {
                    logger.LogText("Invalid arguments. Please use the format '-g -a FrankHuang -k mySecretKey' to generate a new setup code or '-v -k mySecretKey -p 123456' to validate a pin code, or -c to create a new key.");
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
                    SetupCode setupInfo = tfa.GenerateSetupCode("SmartMan2FA", account, secretKey, true);
                    
                    logger.LogText($"Setup code and QRCode for the account '{account}' have been created successfully.");
                    // write the manual entry setup code to a text file
                    string manualEntrySetupCode = setupInfo.ManualEntryKey;
                    // write the QR code setup code to a text file
                    string setupCodeFileName = "setup_code.txt";
                    using (StreamWriter file = new StreamWriter(setupCodeFileName))
                    {
                        file.WriteLine(manualEntrySetupCode);
                    }
                    string setupCodeFullPath = Path.GetFullPath(setupCodeFileName);
                    logger.LogText($"Setup code '{setupCodeFileName}' has been created in '{setupCodeFullPath}'.");
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
                    string qrCodeFullPath = Path.GetFullPath(qrCodeFileName);
                    logger.LogText($"QRCode '{qrCodeFileName}' has been created in '{qrCodeFullPath}'.");
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
                    bool pinCodeIsValid = tfa.ValidateTwoFactorPIN(secretKey, pinCode, true);
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
                else if (createKey)
                {
                    // generate a new secret key
                    var secretKey = TwoStepsAuthenticator.Authenticator.GenerateKey();
                    // write the key to a text file
                    string keyFileName = "secret_key.txt";
                    using (StreamWriter file = new StreamWriter(keyFileName))
                    {
                        file.WriteLine(secretKey);
                    }
                    string keyFullPath = Path.GetFullPath(keyFileName);
                    logger.LogText($"Secret key has been created in '{keyFullPath}'.");
                    return successCode;
                }
                else
                {
                    logger.LogText("Invalid arguments. Please use the format '-g -a FrankHuang -k mySecretKey' to generate a new setup code or '-v -k mySecretKey -p 123456' to validate a pin code, or -c to create a new key.");
                    return errorCode;
                }
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
