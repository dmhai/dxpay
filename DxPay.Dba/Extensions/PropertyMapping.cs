using System.Collections.Generic;
using System.Reflection;

namespace DxPay.Dba.Extensions
{
    public class PropertyMapping
    {
        /// <summary>
        /// ����
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public PropertyInfo[] PropertyInfos { get; set; }
        /// <summary>
        /// �����ֶ�
        /// </summary>
        public string PropertiesString { get; set; }
        /// <summary>
        /// ���Լ���
        /// </summary>
        public List<string> PropertiesList { get; set; }
        /// <summary>
        /// �ֶ�
        /// </summary>
        public string PrimaryKey { get; set; }
    }
}