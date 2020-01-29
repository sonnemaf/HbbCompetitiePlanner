using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace ReflectionIT.Universal.Helpers {

    public static class Extensions {

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex) {
            var aux = list[newIndex];
            list[newIndex] = list[oldIndex];
            list[oldIndex] = aux;
        }

        public static bool Between<T>(this T me, T lower, T upper) where T : IComparable<T> {
            return me.CompareTo(lower) >= 0 && me.CompareTo(upper) <= 0;
        }

        public static System.IO.MemoryStream ToStream(this string source) {
            var bytes = System.Text.Encoding.UTF8.GetBytes(source);
            return new System.IO.MemoryStream(bytes);
        }

        public static Random Random { get; set; } = new Random((int)DateTime.Now.Ticks);

        public static void Shuffle<T>(this IList<T> list) {
            var r = new Random((int)DateTime.Now.Ticks);

            for (var i = list.Count - 1; i > 0; i--) {
                var j = r.Next(0, i);
                var e = list[i];
                list[i] = list[j];
                list[j] = e;
            }
        }

        /// <summary>
        /// Determines whether mutliple (more than 1) elements of a sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The target</typeparam>
        /// <param name="target"> An System.Collections.Generic.IEnumerable`1 that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if more than one element passes the test in the specified predicate</returns>
        public static bool Multiple<T>(this IEnumerable<T> target, Func<T, bool> predicate) {
            var found = false;
            foreach (var item in target) {
                if (predicate(item)) {
                    if (found) {
                        return true;
                    }
                    found = true;
                }
            }
            return false;
        }

        public static IEnumerable<T> Outer<T>(this IEnumerable<T> source, int count) {
            foreach (var item in source.Take(count)) {
                yield return item;
            }
            foreach (var item in source.Reverse().Take(count)) {
                yield return item;
            }
        }

        //public static IEnumerable<T> Append<T>(this IEnumerable<T> target, T obj) {
        //    foreach (var item in target) {
        //        yield return item;
        //    }
        //    yield return obj;
        //}

        //public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, T obj) {
        //    yield return obj;
        //    foreach (var item in target) {
        //        yield return item;
        //    }
        //}

        public static bool CountAtLeast<T>(this IEnumerable<T> target, int minimum, Func<T, bool> predicate) {
            int count = 0;
            foreach (var item in target) {
                if (predicate(item)) {
                    if (++count == minimum) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> target, T excludeItem) where T : class {
            foreach (var item in target) {
                if (item != excludeItem) {
                    yield return item;
                }
            }
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, int capacity) {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");
            }

            var list = new List<TSource>(capacity);
            list.AddRange(source);
            return list;
        }

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison) {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (var i = 0; i < sortableList.Count; i++) {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            if (source is IList<T> list) {
                for (var i = 0; i < list.Count; i++) {
                    var item = list[i];
                    action.Invoke(item);
                }
            } else {
                foreach (var item in source) {
                    action.Invoke(item);
                }
            }
        }

        public static T RandomPick<T>(this IList<T> target) {
            return target[Random.Next(target.Count)];
        }

        public static T RandomPick<T>(this IEnumerable<T> target) {
            return target.ElementAtOrDefault(Random.Next(target.Count()));
        }

#pragma warning disable CA1030 // Use events where appropriate
        public static void RaiseCanExecuteChanged(this System.Windows.Input.ICommand cmd) {
#pragma warning restore CA1030 // Use events where appropriate
            (cmd as IRelayCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        ///  Searches for an element that matches the conditions defined by the specified match, and returns
        ///  the zero-based index of the first occurrence within the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable<T> to filter.</param>
        /// <param name="match">A function to test each element for a condition.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions
        /// defined by match, if found; otherwise, –1.</returns>
        public static int FirstIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> match) {
            if (match is null) {
                throw new ArgumentNullException("match");
            }
            var index = 0;
            foreach (var item in source) {
                if (match(item)) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        ///  Searches for an element that matches the conditions defined by the specified match, and returns
        ///  the zero-based index of the last occurrence within the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable<T> to filter.</param>
        /// <param name="match">A function to test each element for a condition.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions
        /// defined by match, if found; otherwise, –1.</returns>
        public static int LastIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
            var index = 0;
            foreach (var item in source.Reverse()) {
                if (predicate(item)) {
                    return source.Count() - index - 1;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        /// This extension method replaces an item in a collection that implements the IList interface.
        /// </summary>
        /// <typeparam name="T">The type of the field that we are manipulating</typeparam>
        /// <param name="thisList">The input list</param>
        /// <param name="position">The position of the old item</param>
        /// <param name="item">The item we are goint to put in it's place</param>
        /// <returns>True in case of a replace, false if failed</returns>
        public static bool Replace<T>(this IList<T> thisList, int position, T item) {
            if (position > thisList.Count - 1) {
                return false;
            }
            // only process if inside the range of this list

            thisList.RemoveAt(position);
            // remove the old item
            thisList.Insert(position, item);
            // insert the new item at its position
            return true;
            // return success
        }

        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="list">A sequence in which to locate a value.</param>
        /// <param name="value">The object to locate in the sequence</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value) where TSource : IEquatable<TSource> {

            return list.IndexOf<TSource>(value, EqualityComparer<TSource>.Default);
        }

        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="list">A sequence in which to locate a value.</param>
        /// <param name="value">The object to locate in the sequence</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer) {
            var index = 0;
            foreach (var item in list) {
                if (comparer.Equals(item, value)) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static void AddSorted<T>(this IList<T> list, T value) where T : IComparable<T> {
            var val = list.Where(x => x.CompareTo(value) < 0);
            if (val.Count() > 0) {
                var pos = list.IndexOf(val.Last());
                list.Insert(pos + 1, value);
            } else {
                list.Insert(0, value);
            }
        }

        public static void AddSorted<T>(this IList<T> list, T value, IComparer<T> c) {
            var val = list.Where(x => c.Compare(x, value) < 0);
            if (val.Count() > 0) {
                var pos = list.IndexOf(val.Last());
                list.Insert(pos + 1, value);
            } else {
                list.Insert(0, value);
            }
        }

        public static void AddSorted<T>(this IList<T> list, T value, Comparison<T> c) {
            var val = list.Where(x => c(x, value) < 0);
            if (val.Count() > 0) {
                var pos = list.IndexOf(val.Last());
                list.Insert(pos + 1, value);
            } else {
                list.Insert(0, value);
            }
        }

        //[Obsolete("Use ObservableList<T> class instead")]
        public static void AddRange<T>(this Collection<T> oc, IEnumerable<T> collection) {
            if (collection is null) {
                throw new ArgumentNullException("collection");
            }
            foreach (var item in collection) {
                oc.Add(item);
            }
        }

        /// <summary>
        /// Returns the first few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the
        /// given length the complete string is returned. If length is zero or
        /// less an empty string is returned
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="length">Number of characters to return</param>
        public static string Left(this string s, int length) {
            length = Math.Max(length, 0);

            return s.Length > length ? s.Substring(0, length) : s;
        }

        public static int GetWeekNumber(this DateTime dtPassed) {
            var ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            var weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        public static void Sort<T>(this List<T> list, Func<T, T, int> comparison) {
            list.Sort(Comparer<T>.Create(new Comparison<T>(comparison)));
        }


        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items) {
            return items.Count() > 1
                ? items.SelectMany(item => GetPermutations(items.Where(i => !i.Equals(item))),
                                        (item, permutation) => new[] { item }.Concat(permutation))
                : (new[] { items });
        }

        // ---------------------------------------------

       

        public static T GetAttributeValueOrDefault<T>(this System.Xml.Linq.XElement xml, string name, T defaultValue, Func<string, T> convert) {
            var at = xml.Attribute(name);
            return at is null ? defaultValue : convert(at.Value);
        }

        public static T GetAttributeValueOrDefault<T>(this System.Xml.Linq.XElement xml, string name, T defaultValue) {
            var at = xml.Attribute(name);
#pragma warning disable CA1305 // Specify IFormatProvider
            return at is null ? defaultValue : (T)global::System.Convert.ChangeType(at.Value, typeof(T));
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        public static T GetAttributeValue<T>(this System.Xml.Linq.XElement xml, string name) {
            var at = xml.Attribute(name);
#pragma warning disable CA1305 // Specify IFormatProvider
            return (T)global::System.Convert.ChangeType(at.Value, typeof(T));
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        /// <summary>
        /// Returns the last few characters of the string with a length
        /// specified by the given parameter. If the string's length is less than the
        /// given length the complete string is returned. If length is zero or
        /// less an empty string is returned
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="length">Number of characters to return</param>
        public static string Right(this string s, int length) {
            length = Math.Max(length, 0);

            return s.Length > length ? s.Substring(s.Length - length, length) : s;
        }

        public static void Remove<T>(this ICollection<T> list, Func<T, bool> predicate) {
            var items = list.Where(predicate).ToList();

            foreach (var item in items) {
                list.Remove(item);
            }
        }

        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute {
            return enumValue
                .GetType()
                .GetTypeInfo()
                .GetDeclaredField(enumValue.ToString())
                .GetCustomAttribute<T>();
        }

        //public static TAttr GetAttribute<TEnum, TAttr>(this TEnum enumValue) where TAttr : Attribute {
        //    return enumValue.GetType().GetTypeInfo()
        //        .GetDeclaredField(enumValue.ToString())
        //        .GetCustomAttribute<TAttr>();
        //}
    }

    public static class TaskExtensions {

        // Source: https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/Scenarios/Infrastructure/TaskExtensions.cs

#pragma warning disable RIT0002 // Async method should be named with an Async suffix
        /// <summary>
        /// This properly registers and unregisters the token when one of the operations completes
        /// </summary>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken) {
#pragma warning restore RIT0002 // Async method should be named with an Async suffix
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            // This disposes the registration as soon as one of the tasks trigger
            using (cancellationToken.Register(state => {
                ((TaskCompletionSource<object>)state).TrySetResult(null);
            }, tcs)) {
                var resultTask = await Task.WhenAny(task, tcs.Task);
                if (resultTask == tcs.Task) {
                    // Operation cancelled
                    throw new OperationCanceledException(cancellationToken);
                }
                return await task;
            }
        }

#pragma warning disable RIT0002 // Async method should be named with an Async suffix
        /// <summary>
        /// This method cancels the timer if the operation succesfully completes.
        /// Source: https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#always-dispose-cancellationtokensources-used-for-timeouts
        /// </summary>
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout) {
#pragma warning restore RIT0002 // Async method should be named with an Async suffix
            using (var cts = new CancellationTokenSource()) {
                var delayTask = Task.Delay(timeout, cts.Token);

                var resultTask = await Task.WhenAny(task, delayTask);
                if (resultTask == delayTask) {
                    // Operation cancelled
                    throw new OperationCanceledException();
                } else {
                    // Cancel the timer task so that it does not fire
                    cts.Cancel();
                }

                return await task;
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T[,] target) {
            foreach (var item in target) {
                yield return item;
            }
        }

        public static bool IsInSameWeek(this DateTime date1, DateTime date2) {
            return date1.AddDays(-(int)date1.DayOfWeek) == date2.AddDays(-(int)date2.DayOfWeek);
        }


    }
}
