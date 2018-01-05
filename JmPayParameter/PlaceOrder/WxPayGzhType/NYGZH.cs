using JMP.TOOL;
using JmPayParameter.PayChannel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Text;
using DxPay.LogManager.LogFactory.ApiLog;
using JmPayParameter.Models;

namespace JmPayParameter.PlaceOrder.WxPayGzhType
{
    /// <summary>
    /// ��������΢�Ź��ں�ͨ��
    /// </summary>
    public class NYGZH
    {

        /// <summary>
        /// ����΢�Ź��ں�֧��ͨ�������
        /// </summary>
        /// <param name="paymode">ƽ̨���ͣ�1����׿��2��ios��3��H5��</param>
        /// <param name="apptype">������ñ�id</param>
        /// <param name="code">�������</param>
        /// <param name="goodsname">��Ʒ����</param>
        /// <param name="price">��Ʒ�۸񣨵�λԪ��</param>
        /// <param name="orderid">����id</param>
        /// <param name="infoTime">��ѯ�ӿ���Ϣ����ʱ��</param>
        /// <param name="appid">Ӧ��id</param>
        /// <returns></returns>
        public InnerResponse NyWxGzhPayInfo(int paymode, int apptype, string code, string goodsname, decimal price, int orderid, int infoTimes, int appid)
        {
            InnerResponse inn = new InnerResponse();
            if (paymode == 3)
            {
                inn = NyGzhH5(apptype, code, price, orderid, goodsname, appid, infoTimes);
            }
            else
            {
                inn = inn.ToResponse(ErrorCode.Code9987);
            }
            return inn;
        }

        /// <summary>
        /// �������ں�֧��
        /// </summary>
        /// <param name="apptype">Ӧ������id</param>
        /// <param name="code">�������</param>
        /// <param name="price">��Ʒ�۸�</param>
        /// <param name="oid">����id</param>
        /// <param name="goodsname">��Ʒ����</param>
        /// <param name="appid">Ӧ��id</param>
        /// <returns></returns>
        private InnerResponse NyGzhH5(int apptype, string code, decimal price, int orderid, string goodsname, int appid, int infoTimes)
        {
            InnerResponse inn = new InnerResponse();
            SelectInterface SeIn = new SelectInterface();
            try
            {
                string Nywxgzh = "Nywxgzh" + appid;//��װ����keyֵ

                SeIn = SelectUserInfo(Nywxgzh, apptype, appid, infoTimes);
                if (SeIn == null || SeIn.PayId <= 0 || string.IsNullOrEmpty(SeIn.UserId) || string.IsNullOrEmpty(SeIn.UserKey))
                {
                    inn = inn.ToResponse(ErrorCode.Code106);
                    return inn;
                }
                if (!UpdateOrde.OrdeUpdateInfo(orderid, SeIn.PayId))
                {
                    inn = inn.ToResponse(ErrorCode.Code101);
                    return inn;
                }
                if (!JudgeMoney.JudgeMinimum(price, SeIn.minmun))
                {
                    inn = inn.ToResponse(ErrorCode.Code8990);
                    return inn;
                }
                if (!JudgeMoney.JudgeMaximum(price, SeIn.maximum))
                {
                    inn = inn.ToResponse(ErrorCode.Code8989);
                    return inn;
                }
                Dictionary<string, string> strlist = new Dictionary<string, string>();
                strlist.Add("tradeType", "cs.pay.submit");//��������
                strlist.Add("version", "1.3");//�汾��
                strlist.Add("mchId", SeIn.UserId);//�����̺�
                strlist.Add("channel", "wxPub");//֧������
                strlist.Add("body", goodsname);//��Ʒ����
                strlist.Add("outTradeNo", code);//�̻�������
                strlist.Add("amount", price.ToString());//���׽��
                                                        //strlist.Add("description", JMP.TOOL.DESEncrypt.Encrypt(code));//�Զ�����Ϣ
                strlist.Add("notifyUrl", ConfigurationManager.AppSettings["NyNotifyUrl"].ToString().Replace("{0}", SeIn.PayId.ToString()));//�첽֪ͨ
                strlist.Add("callbackUrl", ConfigurationManager.AppSettings["GOTOUrl"].ToString().Replace("{0}", orderid.ToString()));//ͬ��֪ͨ
                string md5str = JMP.TOOL.UrlStr.AzGetStr(strlist) + "&key=" + SeIn.UserKey;
                string md5 = JMP.TOOL.MD5.md5strGet(md5str, true);
                strlist.Add("sign", md5);//ǩ��
                string extra = "";

                strlist.Add("extra", extra);//��չ�ֶ�
                string postString = JMP.TOOL.JsonHelper.DictJsonstr(strlist, "extra");//���ＴΪ���ݵĲ����������ù���ץ��������Ҳ�����Լ���������Ҫ��form����ÿһ��name��Ҫ�ӽ��� 
                byte[] postData = Encoding.UTF8.GetBytes(postString);//���룬�����Ǻ��֣�����Ҫ����ץȡ��ҳ�ı��뷽ʽ  
                string url = ConfigurationManager.AppSettings["NYPOSTUrl"].ToString();//�����ַ  
                WebClient webClient = new WebClient();
                byte[] responseData = webClient.UploadData(url, "POST", postData);//�õ������ַ���  
                string srcString = Encoding.UTF8.GetString(responseData);//����  
                Dictionary<string, object> jsonstr = JMP.TOOL.JsonHelper.DataRowFromJSON(srcString);
                if (jsonstr.ContainsKey("returnCode") && jsonstr["resultCode"].ToString() == "0")
                {

                    inn = inn.ToResponse(ErrorCode.Code100);
                    inn.ExtraData = jsonstr["payCode"].ToString();//http�ύ��ʽ;
                    inn.IsJump = true;
                }
                else
                {
                    string error = "�������ںŽӿڴ�����룺" + srcString + ",�̻��ţ�" + SeIn.UserId;
                    PayApiDetailErrorLogger.UpstreamPaymentErrorLog("������Ϣ��" + error, summary: "�������ںŽӿڴ�����Ϣ", channelId: SeIn.PayId);
                    inn = inn.ToResponse(ErrorCode.Code104);
                }
            }
            catch (Exception E)
            {

                PayApiDetailErrorLogger.UpstreamPaymentErrorLog("������Ϣ��" + E.ToString(), summary: "�������ںŽӿڴ�����Ϣ", channelId: SeIn.PayId);
                inn = inn.ToResponse(ErrorCode.Code104);
            }
            return inn;
        }

        /// <summary>
        /// ��ȡ�����˺���Ϣ
        /// </summary>
        /// <param name="cache">����key</param>
        /// <param name="apptype">������ñ�id</param>
        /// <param name="appid">Ӧ��id</param>
        /// <returns></returns>
        private SelectInterface SelectUserInfo(string cache, int apptype, int appid, int infoTimes)
        {
            SelectInterface SeIn = new SelectInterface();
            try
            {
                DataTable dt = new DataTable();
                JMP.BLL.jmp_interface bll = new JMP.BLL.jmp_interface();
                if (JMP.TOOL.CacheHelper.IsCache(cache))
                {
                    dt = JMP.TOOL.CacheHelper.GetCaChe<DataTable>(cache);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int row = new Random().Next(0, dt.Rows.Count);
                        string[] paypz = dt.Rows[row]["l_str"].ToString().Split(',');
                        SeIn.UserId = paypz[0].Replace("\r", "").Replace("\n", "").Trim();//��ȡ���������е��������ں�id
                        SeIn.UserKey = paypz[1].Replace("\r", "").Replace("\n", "").Trim();//��ȡ���������е��������ں�key
                        SeIn.PayId = int.Parse(dt.Rows[row]["l_id"].ToString());
                        SeIn.minmun = decimal.Parse(dt.Rows[row]["l_minimum"].ToString());//������С֧�����
                        SeIn.maximum = decimal.Parse(dt.Rows[row]["l_maximum"].ToString());//�������֧�����
                    }
                    else
                    {
                        dt = bll.SelectPay("NYGZH", apptype, appid);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int row = new Random().Next(0, dt.Rows.Count);
                            string[] paypz = dt.Rows[row]["l_str"].ToString().Split(',');
                            SeIn.UserId = paypz[0].Replace("\r", "").Replace("\n", "").Trim();//��ȡ�������ں�id
                            SeIn.UserKey = paypz[1].Replace("\r", "").Replace("\n", "").Trim();//��ȡ�������ں�key
                            SeIn.PayId = int.Parse(dt.Rows[row]["l_id"].ToString());
                            SeIn.minmun = decimal.Parse(dt.Rows[row]["l_minimum"].ToString());//������С֧�����
                            SeIn.maximum = decimal.Parse(dt.Rows[row]["l_maximum"].ToString());//�������֧�����
                            JMP.TOOL.CacheHelper.CacheObjectLocak<DataTable>(dt, cache, infoTimes);//���뻺��
                        }
                        else
                        {
                            PayApiDetailErrorLogger.UpstreamPaymentErrorLog("Ӧ��idΪ��" + appid + "��֧��ͨ��Ϊ�գ����idΪ��" + apptype + ",��ȡ����ʧ�ܺ󣬴����ݿ�δ��ѯ�������Ϣ��", summary: "�������ں�֧���ӿڴ���", channelId: SeIn.PayId);
                        }
                    }
                }
                else
                {
                    dt = bll.SelectPay("NYGZH", apptype, appid);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int row = new Random().Next(0, dt.Rows.Count);
                        string[] paypz = dt.Rows[row]["l_str"].ToString().Split(',');
                        SeIn.UserId = paypz[0].Replace("\r", "").Replace("\n", "").Trim();//��ȡ�������ں�id
                        SeIn.UserKey = paypz[1].Replace("\r", "").Replace("\n", "").Trim();//��ȡ�������ں�key
                        SeIn.PayId = int.Parse(dt.Rows[row]["l_id"].ToString());
                        SeIn.minmun = decimal.Parse(dt.Rows[row]["l_minimum"].ToString());//������С֧�����
                        SeIn.maximum = decimal.Parse(dt.Rows[row]["l_maximum"].ToString());//�������֧�����
                        JMP.TOOL.CacheHelper.CacheObjectLocak<DataTable>(dt, cache, infoTimes);//���뻺��
                    }
                    else
                    {
                        PayApiDetailErrorLogger.UpstreamPaymentErrorLog("Ӧ��idΪ��" + appid + "��֧��ͨ��Ϊ�գ����idΪ��" + apptype + ",�����ݿ�δ��ѯ�������Ϣ��", summary: "�������ں�֧���ӿڴ���", channelId: SeIn.PayId);
                    }
                }
            }
            catch (Exception e)
            {
                string bcxx = "������ʾ" + e.Message + "�������" + e.Source + "��������" + e.TargetSite + "������Ϣ��" + e.ToString();//������Ϣ
                PayApiDetailErrorLogger.UpstreamPaymentErrorLog(bcxx, summary: "�������ں�֧���ӿڴ���Ӧ������ID��" + apptype, channelId: SeIn.PayId);
                throw;
            }
            return SeIn;
        }
    }
}
