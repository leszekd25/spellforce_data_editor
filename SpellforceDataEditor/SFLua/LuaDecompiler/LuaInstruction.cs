using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{ // taken from lopcodes.h
    public enum LuaOpCode
    {
        OP_END,/*	-	-		(return)	no results	*/
        OP_RETURN,/*	U	v_n-v_x(at u)	(return)	returns v_x-v_n	*/

        OP_CALL,/*	A B	v_n-v_1 f(at a)	r_b-r_1		f(v1,...,v_n)	*/
        OP_TAILCALL,/*	A B	v_n-v_1 f(at a)	(return)	f(v1,...,v_n)	*/

        OP_PUSHNIL,/*	U	-		nil_1-nil_u			*/
        OP_POP,/*	U	a_u-a_1		-				*/

        OP_PUSHINT,/*	S	-		(Number)s			*/
        OP_PUSHSTRING,/* K	-		KSTR[k]				*/
        OP_PUSHNUM,/*	N	-		KNUM[n]				*/
        OP_PUSHNEGNUM,/* N	-		-KNUM[n]			*/

        OP_PUSHUPVALUE,/* U	-		Closure[u]			*/

        OP_GETLOCAL,/*	L	-		LOC[l]				*/
        OP_GETGLOBAL,/*	K	-		VAR[KSTR[k]]			*/

        OP_GETTABLE,/*	-	i t		t[i]				*/
        OP_GETDOTTED,/*	K	t		t[KSTR[k]]			*/
        OP_GETINDEXED,/* L	t		t[LOC[l]]			*/
        OP_PUSHSELF,/*	K	t		t t[KSTR[k]]			*/

        OP_CREATETABLE,/* U	-		newarray(size = u)		*/

        OP_SETLOCAL,/*	L	x		-		LOC[l]=x	*/
        OP_SETGLOBAL,/*	K	x		-		VAR[KSTR[k]]=x	*/
        OP_SETTABLE,/*	A B	v a_a-a_1 i t	(pops b values)	t[i]=v		*/

        OP_SETLIST,/*	A B	v_b-v_1 t	t		t[i+a*FPF]=v_i	*/
        OP_SETMAP,/*	U	v_u k_u - v_1 k_1 t	t	t[k_i]=v_i	*/

        OP_ADD,/*	-	y x		x+y				*/
        OP_ADDI,/*	S	x		x+s				*/
        OP_SUB,/*	-	y x		x-y				*/
        OP_MULT,/*	-	y x		x*y				*/
        OP_DIV,/*	-	y x		x/y				*/
        OP_POW,/*	-	y x		x^y				*/
        OP_CONCAT,/*	U	v_u-v_1		v1..-..v_u			*/
        OP_MINUS,/*	-	x		-x				*/
        OP_NOT,/*	-	x		(x==nil)? 1 : nil		*/

        OP_JMPNE,/*	J	y x		-		(x~=y)? PC+=s	*/
        OP_JMPEQ,/*	J	y x		-		(x==y)? PC+=s	*/
        OP_JMPLT,/*	J	y x		-		(x<y)? PC+=s	*/
        OP_JMPLE,/*	J	y x		-		(x<y)? PC+=s	*/
        OP_JMPGT,/*	J	y x		-		(x>y)? PC+=s	*/
        OP_JMPGE,/*	J	y x		-		(x>=y)? PC+=s	*/

        OP_JMPT,/*	J	x		-		(x~=nil)? PC+=s	*/
        OP_JMPF,/*	J	x		-		(x==nil)? PC+=s	*/
        OP_JMPONT,/*	J	x		(x~=nil)? x : -	(x~=nil)? PC+=s	*/
        OP_JMPONF,/*	J	x		(x==nil)? x : -	(x==nil)? PC+=s	*/
        OP_JMP,/*	J	-		-		PC+=s		*/

        OP_PUSHNILJMP,/* -	-		nil		PC++;		*/

        OP_FORPREP,/*	J							*/
        OP_FORLOOP,/*	J							*/

        OP_LFORPREP,/*	J							*/
        OP_LFORLOOP,/*	J							*/

        OP_CLOSURE/*	A B	v_b-v_1		closure(KPROTO[a], v_1-v_b)	*/

    }

    public enum LuaArgType { NO_ARG = 0, UNSIGNED1 = 1, SIGNED1 = 2, UNSIGNED2 = 3}

    public class LuaInstruction
    {
        static LuaArgType[] i_arg_type =
        {
             LuaArgType.NO_ARG, LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED2, LuaArgType.UNSIGNED2,
             LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1, LuaArgType.SIGNED1, LuaArgType.UNSIGNED1,
             LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1,
             LuaArgType.UNSIGNED1, LuaArgType.NO_ARG, LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1,
             LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1, LuaArgType.UNSIGNED1,
             LuaArgType.UNSIGNED2, LuaArgType.UNSIGNED2, LuaArgType.UNSIGNED1, LuaArgType.NO_ARG,
             LuaArgType.SIGNED1, LuaArgType.NO_ARG, LuaArgType.NO_ARG, LuaArgType.NO_ARG,
             LuaArgType.NO_ARG, LuaArgType.UNSIGNED1, LuaArgType.NO_ARG, LuaArgType.NO_ARG,
             LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1,
             LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1,
             LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1,  LuaArgType.NO_ARG,
             LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1, LuaArgType.SIGNED1,
             LuaArgType.UNSIGNED2

        };
        uint data;

        public LuaInstruction(uint u)
        {
            data = u;
        }

        public LuaOpCode OpCode { get { return (LuaOpCode)(data&0x3F); } }
        public int ArgU { get { return (int)(data >> 6); } }
        public int ArgS { get { return (int)(ArgU - (Int32.MaxValue>>6)); } }
        public int ArgA { get { return (int)(data >> 15); } }
        public int ArgB { get { return (int)((data<<17)>>23); } }

        public override string ToString()
        {
            LuaArgType t = i_arg_type[(int)OpCode];
            if (t == LuaArgType.NO_ARG)
                return OpCode.ToString() + "()";
            if (t == LuaArgType.SIGNED1)
                return OpCode.ToString() + "(" + ArgS.ToString() + ")";
            if (t == LuaArgType.UNSIGNED1)
                return OpCode.ToString() + "(" + ArgU.ToString() + ")";
            if (t == LuaArgType.UNSIGNED2)
                return OpCode.ToString() + "( " + ArgA.ToString() + ", " + ArgB.ToString() + ")";
            return Utility.S_MISSING;
        }
    }
}
