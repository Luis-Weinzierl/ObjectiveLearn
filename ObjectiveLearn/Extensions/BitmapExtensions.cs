using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveLearn.Extensions;

public static class BitmapExtensions
{
	public static Eto.Drawing.Image ToIcon(this System.Drawing.Bitmap bitmap)
	{
		using var iconStream = new MemoryStream();

		bitmap.Save(iconStream, bitmap.RawFormat);
		iconStream.Position = 0;

		var icon = new Eto.Drawing.Bitmap(iconStream);

        return icon;
	}
}
