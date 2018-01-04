using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardDemo
{
    public class KeyEventNode : TreeNode
    {

        private TreeNode keyDownNode;
        private TreeNode keyPressedNode;
        private TreeNode keyUpNode;

//        public bool Checked                 { get { return this.Checked; } }
        public bool KeyDownNodeChecked      { get { return keyDownNode.Checked; } }
        public bool KeyPressedNodeChecked   { get { return keyPressedNode.Checked; } }
        public bool KeyUpNodeChecked        { get { return keyUpNode.Checked; } }


        /// <summary>
        /// Construct a root note containing 3 child nodes
        /// </summary>
        /// <param name="name"></param>
        public KeyEventNode(string name)
        {
            bool initialValue = true;
            this.Text = name;
            keyDownNode     = new TreeNode("KeyDown");
            keyPressedNode  = new TreeNode("KeyPressed");
            keyUpNode       = new TreeNode("KeyUp");
            Nodes.Add(keyDownNode);
            Nodes.Add(keyPressedNode);
            Nodes.Add(keyUpNode);
            this.Checked = initialValue;
            keyDownNode.Checked = initialValue;
            keyPressedNode.Checked = initialValue;
            keyUpNode.Checked = false;
        }
    }
}
