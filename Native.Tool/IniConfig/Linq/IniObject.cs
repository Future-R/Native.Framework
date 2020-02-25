using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Native.Tool.IniConfig.Linq
{
	/// <summary>
	/// 用于描述 Ini 配置项的类
	/// </summary>
	[Serializable]
	public class IniObject : ICollection<IniSection>, IReadOnlyCollection<IniSection>
	{
		#region --字段--
		private SortedDictionary<string, IniSection> _sections;
		private string _filePath = string.Empty;
		private Encoding _encoding = Encoding.Default;
		private static readonly Lazy<Regex[]> regices = new Lazy<Regex[]> (() => new Regex[]
		{
			new Regex(@"^\[(.+)\]", RegexOptions.Compiled),						//匹配 节
			new Regex(@"^([^\r\n=]+)=((?:[^\r\n]+)?)",RegexOptions.Compiled),    //匹配 键值对
 			new Regex(@"^;(?:[\s\S]*)", RegexOptions.Compiled)				//匹配 注释
		});
		#endregion

		#region --属性--
		/// <summary>
		/// 获取指定索引处的元素
		/// </summary>
		/// <param name="index">要获取或设置的元素的从零开始的索引</param>
		/// <returns>指定索引处的元素</returns>
		public IniSection this[string key]
		{
			get
			{
				return this._sections[key];
			}
		}

		/// <summary>
		/// 获取 <see cref="IniObject"/> 中包含的元素数
		/// </summary>
		public int Count { get { return this._sections.Count; } }

		/// <summary>
		/// 获取一个值，该值指示 <see cref="IniObject"/> 是否为只读
		/// </summary>
		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// 获取或设置用于读取或保存 Ini 配置项的 <see cref="System.Text.Encoding"/> 实例, 默认: ANSI
		/// </summary>
		public Encoding Encoding { get { return this._encoding; } set { this._encoding = value; } }

		/// <summary>
		/// 获取或设置用于保存 Ini 配置项的 <see cref="Uri"/> 实例
		/// </summary>
		public Uri Path { get; set; }

		/// <summary>
		/// 获取用于解析 Ini 配置项的 Regex 对象数组
		/// </summary>
		private static Regex[] Regices { get { return regices.Value; } }
		#endregion

		#region --构造函数--
		/// <summary>
		/// 初始化 <see cref="IniObject"/> 类的新实例，该实例为空并且具有默认初始容量
		/// </summary>
		[TargetedPatchingOptOut ("性能至关重要的内联跨NGen图像边界")]
		public IniObject ()
		{
			this._sections = new SortedDictionary<string, IniSection> ();
		}

		/// <summary>
		/// 初始化 <see cref="IniObject"/> 类的新实例，该实例包含从指定集合复制的元素并且具有足够的容量来容纳所复制的元素
		/// </summary>
		/// <param name="collection">一个集合，其元素被复制到新列表中</param>
		/// <exception cref="ArgumentNullException">参数: collection 为 null</exception>
		public IniObject (IEnumerable<IniSection> collection)
			: this ()
		{
			if (collection == null)
			{
				throw new ArgumentNullException ("collection");
			}

			foreach (IniSection section in collection)
			{
				this._sections.Add (section.Name, section);
			}
		}
		#endregion

		#region --公开方法--
		/// <summary>
		/// 将某项添加到 <see cref="IniObject"/> 中
		/// </summary>
		/// <param name="item">要添加到 <see cref="IniObject"/> 的对象</param>
		/// <exception cref="ArgumentNullException">参数: item 为 null</exception>
		public void Add (IniSection item)
		{
			if (item == null)
			{
				throw new ArgumentNullException ("item");
			}

			this._sections.Add (item.Name, item);
		}

		/// <summary>
		/// 确定 <see cref="IniObject"/> 是否包含特定值
		/// </summary>
		/// <param name="item">要在 <see cref="IniObject"/> 中定位的对象</param>
		/// <exception cref="ArgumentNullException">参数: item 为 null</exception>
		/// <returns>如果在 true 中找到 item，则为 <see cref="IniObject"/> 否则为 false</returns>
		public bool Contains (IniSection item)
		{
			if (item == null)
			{
				throw new ArgumentNullException ("item");
			}

			return this._sections.ContainsValue (item);
		}

		/// <summary>
		/// 确定是否 <see cref="IniObject"/> 包含带有指定键的元素
		/// </summary>
		/// <param name="key">要在 <see cref="IniObject"/> 中定位的键</param>
		/// <exception cref="ArgumentNullException">参数: key 为 null</exception>
		/// <returns>如果 true 包含具有指定键的元素，则为 <see cref="IniObject"/> 否则为 false</returns>
		public bool ContainsKey (string key)
		{
			return this._sections.ContainsKey (key);
		}

		/// <summary>
		/// 从特定的 <see cref="IniObject"/> 索引处开始，将 <see cref="Array"/> 的元素复制到一个 <see cref="Array"/> 中
		/// </summary>
		/// <param name="array">一维 <see cref="Array"/>，它是从 <see cref="IniObject"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引</param>
		/// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
		/// <exception cref="ArgumentNullException">参数: array 为 null</exception>
		/// <exception cref="ArgumentOutOfRangeException">参数: arrayIndex 小于 0</exception>
		/// <exception cref="ArgumentException">源中的元素数目 <see cref="IniObject"/> 大于从的可用空间 index 目标从头到尾 array</exception>
		public void CopyTo (IniSection[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException ("array");
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException ("arrayIndex");
			}

			this._sections.Values.CopyTo (array, arrayIndex);
		}

		/// <summary>
		/// 从 <see cref="IniObject"/> 中移除带有指定键的元素
		/// </summary>
		/// <param name="key">要移除的元素的键</param>
		/// <exception cref="ArgumentNullException">参数: key 为 null</exception>
		/// <returns>如果从 true 中成功移除 item，则为 <see cref="IniObject"/> 否则为 false。 如果在原始 false 中没有找到 item，该方法也会返回 <see cref="IniObject"/></returns>
		public bool Remove (string key)
		{
			if (string.IsNullOrEmpty (key))
			{
				throw new ArgumentNullException ("key");
			}
			return this._sections.Remove (key);
		}

		/// <summary>
		/// 从 <see cref="IniObject"/> 中移除特定对象的第一个匹配项
		/// </summary>
		/// <param name="item">要从 <see cref="IniObject"/> 中删除的对象</param>
		/// <exception cref="ArgumentNullException">参数: item 为 null</exception>
		/// <returns>如果从 true 中成功移除 item，则为 <see cref="IniObject"/> 否则为 false。 如果在原始 false 中没有找到 item，该方法也会返回 <see cref="IniObject"/></returns>
		public bool Remove (IniSection item)
		{
			if (item == null)
			{
				throw new ArgumentNullException ("item");
			}

			if (!this._sections.ContainsKey (item.Name))
			{
				return false;
			}

			if (!this._sections[item.Name].Equals (item))
			{
				return false;
			}

			return this._sections.Remove (item.Name);
		}

		/// <summary>
		/// 从 <see cref="IniObject"/> 中移除所有项
		/// </summary>
		public void Clear ()
		{
			this._sections.Clear ();
		}

		/// <summary>
		/// 获取与指定键关联的值
		/// </summary>
		/// <param name="key">要获取的值的键</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值</param>
		/// <exception cref="ArgumentNullException">参数: key 为 null</exception>
		/// <returns>如果 true 包含具有指定键的元素，则为 <see cref="IniObject"/> 否则为 false</returns>
		public bool TryGetValue (string key, out IniSection value)
		{
			return this._sections.TryGetValue (key, out value);
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器
		/// </summary>
		/// <returns>用于循环访问集合的枚举数</returns>
		public IEnumerator<IniSection> GetEnumerator ()
		{
			return this._sections.Values.GetEnumerator ();
		}

		/// <summary>
		/// 返回循环访问集合的枚举数
		/// </summary>
		/// <returns>一个可用于循环访问集合的 <see cref="IEnumerable"/> 对象</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this._sections.Values.GetEnumerator ();
		}

		/// <summary>
		/// 将 Ini 配置项保存到指定的文件。如果存在指定文件，则此方法会覆盖它
		/// </summary>
		public void Save ()
		{
			if (this.Path == null)
			{
				throw new UriFormatException (string.Format ("Uri: {0}, 是无效的 Uri 对象", "IniObject.Path"));
			}
			this.Save (this.Path);
		}

		/// <summary>
		/// 将 Ini 配置项保存到指定的文件。 如果存在指定文件，则此方法会覆盖它。
		/// </summary>
		/// <param name="filePath">要将文档保存到其中的文件的位置。</param>
		public void Save (string filePath)
		{
			this.Save (new Uri (filePath));
		}

		/// <summary>
		/// 将 Ini 配置项保存到指定的文件。 如果存在指定文件，则此方法会覆盖它。
		/// </summary>
		/// <param name="fileUri">要将文档保存到其中的文件的位置。</param>
		public virtual void Save (Uri fileUri)
		{
			fileUri = ConvertAbsoluteUri (fileUri);

			if (!fileUri.IsFile)
			{
				throw new ArgumentException ("所指定的必须是文件 URI", "fileUri");
			}

			using (TextWriter textWriter = new StreamWriter (fileUri.GetComponents (UriComponents.Path, UriFormat.Unescaped), false, this.Encoding))
			{
				foreach (IniSection section in this)
				{
					textWriter.WriteLine ("[{0}]", section.Name);
					foreach (KeyValuePair<string, IniValue> pair in section)
					{
						textWriter.WriteLine ("{0}={1}", pair.Key, pair.Value);
					}
					textWriter.WriteLine ();
				}
			}
		}

		/// <summary>
		/// 从文件以 ANSI 编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IniObject LoadOrCreate (string filePath)
		{
			return LoadOrCreate (new Uri (filePath));
		}

		/// <summary>
		/// 从文件以 ANSI 编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="fileUri">文件路径的 Uri 对象</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IniObject LoadOrCreate (Uri fileUri)
		{
			return LoadOrCreate (fileUri, Encoding.Default);
		}

		/// <summary>
		/// 从文件以指定编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="filePath">文件路径字符串</param>
		/// <param name="encoding">文件编码</param>
		/// <returns></returns>
		public static IniObject LoadOrCreate (string filePath, Encoding encoding)
		{
			return LoadOrCreate (new Uri (filePath), encoding);
		}

		/// <summary>
		/// 从文件以指定编码创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="fileUri">文件路径的 Uri 对象</param>
		/// <param name="encoding">文件编码</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IniObject LoadOrCreate (Uri fileUri, Encoding encoding)
		{
			fileUri = ConvertAbsoluteUri (fileUri);

			if (!fileUri.IsFile)
			{
				throw new ArgumentException ("所指定的必须是文件 URI", "fileUri");
			}

			string path = fileUri.GetComponents (UriComponents.Path, UriFormat.Unescaped);

			if (!File.Exists (path))
			{
				File.WriteAllText (path, string.Empty, encoding);
			}

			//解释 Ini 文件
			using (TextReader textReader = new StreamReader (path, encoding))
			{
				IniObject iObj = ParseIni (textReader);
				iObj.Path = fileUri;
				return iObj;
			}
		}

		/// <summary>
		/// 从字符串中创建一个新的 IniObject 实例对象
		/// </summary>
		/// <param name="iniStr">源字符串</param>
		/// <returns>转换成功返回 IniObject 实例对象</returns>
		public static IniObject Parse (string iniStr)
		{
			using (TextReader textReader = new StringReader (iniStr))
			{
				return ParseIni (textReader);
			}
		}
		#endregion

		#region --私有方法--
		/// <summary>
		/// 处理 <see cref="Uri"/> 实例, 将其变为可直接使用的对象
		/// </summary>
		/// <param name="fileUri">文件路径的 <see cref="Uri"/> 对象</param>
		/// <returns>返回处理过的 Uri</returns>
		private static Uri ConvertAbsoluteUri (Uri fileUri)
		{
			if (!fileUri.IsAbsoluteUri)
			{
				// 处理原始字符串
				StringBuilder urlBuilder = new StringBuilder (fileUri.OriginalString);
				urlBuilder.Replace ("/", "\\");
				while (urlBuilder[0] == '\\')
				{
					urlBuilder.Remove (0, 1);
				}

				// 将相对路径转为绝对路径
				urlBuilder.Insert (0, AppDomain.CurrentDomain.BaseDirectory);
				fileUri = new Uri (urlBuilder.ToString (), UriKind.Absolute);
			}

			return fileUri;
		}

		/// <summary>
		/// 逐行解析 Ini 配置文件字符串
		/// </summary>
		/// <param name="textReader"></param>
		/// <returns></returns>
		private static IniObject ParseIni (TextReader textReader)
		{
			IniObject iniObj = new IniObject ();
			IniSection iniSect = null;
			while (textReader.Peek () != -1)
			{
				string line = textReader.ReadLine ();
				if (string.IsNullOrEmpty (line) == false && Regices[2].IsMatch (line) == false)     //跳过空行和注释
				{
					Match match = Regices[0].Match (line);
					if (match.Success)
					{
						iniSect = new IniSection (match.Groups[1].Value);
						iniObj.Add (iniSect);
						continue;
					}

					match = Regices[1].Match (line);
					if (match.Success)
					{
						iniSect.Add (match.Groups[1].Value.Trim (), match.Groups[2].Value);
					}
				}
			}
			return iniObj;
		}
		#endregion

		#region --重写方法--
		/// <summary>
		/// 将当前实例转换为等效的字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			StringBuilder iniString = new StringBuilder ();
			using (TextWriter textWriter = new StringWriter (iniString))
			{
				foreach (IniSection section in this)
				{
					textWriter.WriteLine ("[{0}]", section.Name.Trim ());
					foreach (KeyValuePair<string, IniValue> pair in section)
					{
						textWriter.WriteLine ("{0}={1}", pair.Key.Trim (), pair.Value.Value.Trim ());
					}
					textWriter.WriteLine ();
				}
			}
			return iniString.ToString ();
		}
		#endregion
	}
}
