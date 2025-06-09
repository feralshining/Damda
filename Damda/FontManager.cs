using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace Damda
{
    /// <summary>
    /// 메모리에서 커스텀 폰트를 로드하고 전역으로 사용할 수 있게 해주는 관리자 클래스입니다.
    /// </summary>
    public class FontManager
    {
        private static FontManager instance = new FontManager();
        public PrivateFontCollection privateFont = new PrivateFontCollection();

        /// <summary>
        /// 메모리에서 로딩된 폰트들의 FontFamily 배열입니다.
        /// </summary>
        public static FontFamily[] fontFamilys { get => instance.privateFont.Families; }

        public static readonly Font CookieRun_10 = GetFont("CookieRun", 10.8f);
        public static readonly Font CookieRun_12 = GetFont("CookieRun", 12f);
        public static readonly Font CookieRun_22 = GetFont("CookieRun", 22.2f);
        public static readonly Font Kookmin_12 = GetFont("Kookmin", 12f);

        /// <summary>
        /// 생성자: 리소스에서 폰트를 읽어 메모리에 등록합니다.
        /// </summary>
        public FontManager() => AddFontFromMemory();

        /// <summary>
        /// Properties.Resources에서 폰트 데이터를 읽어 메모리에 로드합니다.
        /// </summary>
        private void AddFontFromMemory()
        {
            List<byte[]> fonts = new List<byte[]>
            {
                Properties.Resources.CookieRun_Regular,       // 쿠키런 보통
                Properties.Resources.KookminJeongeum_Regular  // 국민연금체 보통
            };

            foreach (byte[] font in fonts)
            {
                IntPtr fontBuffer = Marshal.AllocCoTaskMem(font.Length);
                Marshal.Copy(font, 0, fontBuffer, font.Length);
                privateFont.AddMemoryFont(fontBuffer, font.Length);
                Marshal.FreeHGlobal(fontBuffer); // 메모리 해제
            }
        }

        /// <summary>
        /// 로딩된 폰트 중 이름에 keyword가 포함된 폰트를 찾아 Font 객체로 반환합니다.
        /// </summary>
        /// <param name="keyword">폰트 이름 일부 (예: "CookieRun")</param>
        /// <param name="size">폰트 크기</param>
        /// <param name="style">폰트 스타일</param>
        /// <returns>Font 객체 (없으면 시스템 기본 폰트 반환)</returns>
        public static Font GetFont(string keyword, float size, FontStyle style = FontStyle.Regular)
        {
            FontFamily found = null;

            // 반복문으로 폰트 이름 검색
            FontFamily[] families = fontFamilys;
            for (int i = 0; i < families.Length; i++)
            {
                if (families[i].Name.Contains(keyword))
                {
                    found = families[i];
                    break;
                }
            }
            return (found != null) ? new Font(found, size, style) : SystemFonts.DefaultFont;
        }
    }
}
