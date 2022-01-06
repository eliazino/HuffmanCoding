using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Configurations {
    public class SystemVariables {
        public string jwtsecret { get; set; }
        public string dayCodeDictionary { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string environmentName { get; set; }
        public string siteRoot { get; set; }
        public bool debug { get; set; }
        public bool identityExpires { get; set; } = true;
        public int identityExpiryMins { get; set; } = 1440; //1 Day
        public DBConfig MySQL { get; set; }
        public DBConfig SQLite { get; set; }
        public DBConfig MongoDB { get; set; }
        public EmailParam EmailParam { get; set; }
        public Termii Termii { get; set; }
        public QueueServer QueueServer { get; set; }        
        public List<KeysTemplate> PasswordSalt { get; set; }
        public ElasticSearch ElasticSearch { get; set; }
        public WemaBankWalletService WemaBankWalletService { get; set; }
        public AppConfig AppConfig { get; set; }
        public WalletTransferConfig WalletTransferConfig { get; set; }
        public MPayConfig BusPaymentConfig { get; set; }
        public ReloadlyConfig ReloadlyConfig { get; set; }
        public AirtimePaymentConfig AirtimePaymentConfig { get; set; }
        public CoralPayConfig CoralPayConfig { get; set; }
        public AgogoConfig AgogoConfig { get; set; }
        public MerchantPayConfig MerchantPayConfig { get; set; }
        public MPayConfig InterStatePaymentConfig { get; set; }
        public Redis Redis { get; set; }
        public DayPassConfig DayPassConfig { get; set; }
        public TopupCallback WemaCallback { get; set; }
        public TopupCallback MPGCallback { get; set; }
        public TopupCallback AccessCallback { get; set; }
        public AccessVirtualAccount AccessVirtualAccount { get; set; }
    }
    public class DBConfig {
        public string server { get; set; }
        public string port { get; set; }
        public string database { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    public class EmailParam {
        public string fromAddress { get; set; }
        public string fromName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string smtpServer { get; set; }
        public int smtpPort { get; set; }
    }
    public class Termii {
        public string root { get; set; }
        public string sendSMS { get; set; }
        public string sendOTP { get; set; }
        public string verify { get; set; }
        public string api_key { get; set; }
        public string senderID { get; set; }
    }
    public class QueueServer {
        public string mqhost { get; set; }
        public string mquser { get; set; }
        public string mqpw { get; set; }
        public Jobs Jobs { get; set; }
    }

    public class Jobs {
        public string emailTrigger { get; set; }
        public string Registration { get; set; }
        public string Transaction { get; set; }
        public string BusTransaction { get; set; }
        public string Errlog { get; set; }
    }

    public class KeysTemplate {
        public string salt { get; set; }
        public int saltIndex { get; set; }
    }
    public class ElasticSearch {
        public BasicAuthentication BasicAuthentication { get; set; }
        public string[] nodes { get; set; }
        public ApiKeyAuthentication ApiKeyAuthentication { get; set; }
    }
    public class BasicAuthentication {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class ApiKeyAuthentication {
        public string id { get; set; }
        public string apiKey { get; set; }
    }

    public class WemaBankWalletService {
        public string accountExtension { get; set; }
    }
    public class AppConfig {
        public int maxGetDays { get; set; }
        public int maxGetRows { get; set; }
    }
    public class WalletTransferConfig {
        public double leastValue { get; set; }
        public double maxValue { get; set; }
    }
    public class MPayConfig {
        public string beneficiaryIssuer { get; set; }
        public string payoutIssuer { get; set; }
        public string agentID { get; set; }
        public string cashierID { get; set; }
        public string transIDPrefix { get; set; }
        public bool paymentRedeemable { get; set; }
        public int redeemCoolOffDays { get; set; }
    }
    public class AirtimePaymentConfig {
        public string beneficiaryIssuer { get; set; }
        public string payoutIssuer { get; set; }
        public string agentID { get; set; }
        public string cashierID { get; set; }
        public string transIDPrefix { get; set; }
        public bool paymentRedeemable { get; set; }
        public int redeemCoolOffDays { get; set; }
        public double maxPurchase { get; set; }
        public double minPurchase { get; set; }
    }
    public class ReloadlyConfig {
        public string APIClientID { get; set; }
        public string APIClientSecret { get; set; }
        public string audience { get; set; }
        public ReloadlyEndPoints ReloadlyEndPoints { get; set; }
    }
    public class CoralPayConfig {
        public string rootUrl { get; set; }
        public string basicUername { get; set; }
        public string basicPassword { get; set; }
        public string AirtimeVendorSlug { get; set; }
        public List<CoralPaySupportedAirtimeVendors> CoralPaySupportedAirtimeVendors { get; set; }
        public CoralPayEndPoints CoralPayEndPoints { get; set; }
    }
    public class ReloadlyEndPoints {
        public string getOperators { get; set; }
        public string topup { get; set; }
        public string authenticate { get; set; }
    }

    public class CoralPaySupportedAirtimeVendors {
        public string icon { get; set; }
        public string name { get; set; }
    }

    public class CoralPayEndPoints {
        public string billersGroup { get; set; }
        public string packages { get; set; }
        public string postTransactions { get; set; }
    }
    public class AgogoConfig {
        public string endpointUrl { get; set; }
        public string categoryID { get; set; }
    }
    public class MerchantPayConfig {
        public string beneficiary { get; set; }
    }
    public class Redis {
        public string password { get; set; }
        public bool allowAdmin { get; set; }
        public bool ssl { get; set; }
        public int connectTimeout { get; set; }
        public int connectRetry { get; set; }
        public int database { get; set; }
        public List<RedisHost> Hosts { get; set; }
    }
    public class RedisHost {
        public string host { get; set; }
        public int port { get; set; }
    }
    public class DayPassConfig {
        public string dataFile { get; set; }
        public string beneficiaryIssuer { get; set; }
        public string payoutIssuer { get; set; }
        public string agentID { get; set; }
        public string cashierID { get; set; }
        public string transIDPrefix { get; set; }
    }
    public class TopupCallback {
        public string IPRange { get; set; }
        public string XKey { get; set; }
        public string basicUsername { get; set; }
        public string basicPassword { get; set; }
        public string verificationURL { get; set; }
        public string exceptionKeyWord { get; set; }
        public bool verifyClaim { get; set; } = false;
        public string type { get; set; } = "3";
        public object CarrierSpecifics { get; set; } = null;
        public bool verifyIP(string IP) {
            if (this.IPRange == "*")
                return true;
            if (this.IPRange.Contains(',')) { //It's comma delimited
                List<string> ips = new List<string>(this.IPRange.Split(','));
                return ips.Contains(IP);
            }
            if (this.IPRange.Contains('-')) { //It's a range of IPs
                string[] ranges = this.IPRange.Split('-');
                long lowerRange = long.Parse(ranges[0].Replace(".", ""));
                long upperRange = long.Parse(ranges[1].Replace(".", ""));
                long ip = long.Parse(IP.Replace(".", ""));
                return ip >= lowerRange && ip <= upperRange;
            }
            if (IPRange == IP)
                return true;
            return false;
        }
    }
    public class AccessVirtualAccount {
        public string authorization { get; set; }
        public string subKey { get; set; }
        public string merchantID { get; set; }
        public string channelCode { get; set; }
        public string rootUrl { get; set; }
        public AccessEndpoints Endpoints { get; set; }
    }
    public class AccessEndpoints {
        public string createAccount { get; set; }
        public string verifyClaim { get; set; }
    }
}