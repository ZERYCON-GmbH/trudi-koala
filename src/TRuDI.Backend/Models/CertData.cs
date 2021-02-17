namespace TRuDI.Backend.Models
{
    using System;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    using IVU.Http.Http;

    public class CertData
    {
        public CertData(string filename, byte[] data)
        {
            this.FileName = filename;
            this.Data = data;
        }

        public CertData(X509Certificate2 cert)
        {
            this.Init(cert);
        }

        public string FileName { get; }
        public byte[] Data { get; }

        public string Issuer { get; private set; }
        public string Subject { get; private set; }
        public string ValidTimeRange { get; private set; }


        public string Password { get; private set; }

        public CertPasswordState PasswordState { get; set; }

        public CertPasswordState VerifyPassword(string password)
        {
            try
            {
                var cert = new ClientCertificateWithKey(this.Data, string.Empty);
                this.Init(cert.Certificate);
                this.Password = string.Empty;

                this.PasswordState = CertPasswordState.WithoutPassword;
                return CertPasswordState.WithoutPassword;
            }
            catch (Exception)
            {
            }

            try
            {
                var cert = new ClientCertificateWithKey(this.Data, password);
                this.Init(cert.Certificate);
                this.Password = password;

                this.PasswordState = CertPasswordState.PasswordValid;
                return CertPasswordState.PasswordValid;
            }
            catch (AuthenticationException)
            {
                this.PasswordState = CertPasswordState.InvalidPassword;
                return CertPasswordState.InvalidPassword;
            }
            catch (Exception)
            {
                this.PasswordState = CertPasswordState.InvalidCertFile;
                return CertPasswordState.InvalidCertFile;
            }
        }

        private void Init(X509Certificate2 cert)
        {
            this.Subject = cert.GetNameInfo(X509NameType.SimpleName, false);
            this.Issuer = cert.GetNameInfo(X509NameType.SimpleName, true);
            this.ValidTimeRange = $"{cert.NotBefore} bis {cert.NotAfter}";
        }
    }
}
