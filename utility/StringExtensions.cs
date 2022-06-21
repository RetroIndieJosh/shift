// SHIFT - a cross-platform toolkit for streamlined, scripted text adventures
// Copyright (C) 2022 Joshua D McLean
//
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program as LICENSE.txt. If not, see <https://www.gnu.org/licenses/>.

namespace shift
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string str, string oldStr, string newStr)
        {
            var start = str.IndexOf(oldStr);
            if (start < 0)
                return str;
            return $"{str[..start]}{newStr}{str[(start + oldStr.Length)..]}";
        }
    }
}
