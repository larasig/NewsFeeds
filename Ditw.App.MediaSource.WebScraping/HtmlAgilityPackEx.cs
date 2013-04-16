using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Ditw.App.MediaSource.WebScraping
{
    public static class HtmlAgilityPackEx
    {
        public static IEnumerable<HtmlNode> GetOffspringNodesOfType(
            this HtmlNode node,
            String nodeType,
            Int32 depth)
        {
            return GetOffspringNodes(node, nodeType, depth,
                (n, t) => { return n.Name == t; }
                );
        }

        public static HtmlNode GetOffspringNodeWithLongestContent(
            this HtmlNode node,
            String nodeType,
            Int32 depth)
        {
            HtmlNode contentNode = null;
            //Int32 maxCount = -1;
            Int32 maxLength = -1;
            foreach (var n in GetOffspringNodes(node, nodeType, depth,
                (n, t) =>
                {
                    return n.ChildNodes.Where(cn => cn.Name == nodeType).Count() > 0;
                }
                ))
            {
                Int32 len = n.ChildNodes.Where(cn => cn.Name == nodeType)
                    .Sum(cn => cn.InnerText.Length);
                if (maxLength < len)
                {
                    contentNode = n;
                    maxLength = len;
                }
            }
            return contentNode;
        }

        public static IEnumerable<HtmlNode> GetOffspringNodesWithIdStartsWith(
            this HtmlNode node,
            String idPrefix,
            Int32 depth)
        {
            return GetOffspringNodes(node, idPrefix, depth,
                (n, idpre) => { return !String.IsNullOrEmpty(n.Id) && n.Id.StartsWith(idpre); }
                );
        }

        public static IEnumerable<HtmlNode> GetOffspringNodesWithIdLike(
            this HtmlNode node,
            String idRegex,
            Int32 depth)
        {
            return GetOffspringNodes(node, idRegex, depth, NodesWithIdLike);
        }

        private static Boolean NodesWithIdLike(
            HtmlNode node,
            String idRegex
            )
        {
            Regex regex = new Regex(idRegex);
            return !String.IsNullOrEmpty(node.Id) && regex.IsMatch(node.Id);
        }

        private static Boolean NodesWithIdStartsWith(
            HtmlNode node,
            String idPrefix
            )
        {
            return !String.IsNullOrEmpty(node.Id) && node.Id.StartsWith(idPrefix, StringComparison.Ordinal);
        }
            

        public static IEnumerable<HtmlNode> GetOffspringNodes(
            this HtmlNode node,
            String param1,
            Int32 depth,
            Func<HtmlNode, String, Boolean> nodeFilteringFunc)
        {
            List<HtmlNode> result = new List<HtmlNode>();
            //Regex regex = new Regex(idRegex);

            Stack<HtmlNode> nodeStack = new Stack<HtmlNode>();

            nodeStack.Push(node);
            Stack<HtmlNode> nodeStackNew = new Stack<HtmlNode>();
            for (Int32 i = 0; i <= depth; i++)
            {
                while (nodeStack.Count != 0)
                {
                    var n = nodeStack.Pop();
                    if (n.HasChildNodes)
                    {
                        foreach (var child in n.ChildNodes)
                        {
                            nodeStackNew.Push(child);
                        }
                    }
                    if (nodeFilteringFunc(n, param1))
                    {
                        result.Add(n);
                    }
                }
                nodeStack = new Stack<HtmlNode>(nodeStackNew);
                nodeStackNew.Clear();
            }
            return result;
        }
    }
}
