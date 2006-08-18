//
// cfold.cs: Constant Folding
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) 2002 Ximian, Inc.
//

using System;

namespace Mono.MonoBASIC {

	public class ConstantFold {

		 static public Constant ConvertNothingToDefaultConst (EmitContext ec, 
                                                                        Type target_type, Location loc)
                {
                        switch (Type.GetTypeCode (target_type)) {
                        case TypeCode.Boolean  :
                                return new BoolConstant (false);
                        case TypeCode.Byte  :
                                return new ByteConstant (0);
                        case TypeCode.Char  :
                                return new CharConstant ((char)0);
                        case TypeCode.SByte :
                                return new SByteConstant (0);
                        case TypeCode.Int16 :
                                return new ShortConstant (0);
                        case TypeCode.Int32 :
                                return new IntConstant (0);
                        case TypeCode.Int64 :
                                return new LongConstant (0);
                        case TypeCode.Decimal :
                                return new DecimalConstant (System.Decimal.Zero);
                        case TypeCode.Single :
                                return new FloatConstant (0.0F);
                        case TypeCode.Double :
				 return new DoubleConstant (0.0);
                        }

                        return null;
                }


		//
		// Performs the numeric promotions on the left and right expresions
		// and desposits the results on `lc' and `rc'.
		//
		// On success, the types of `lc' and `rc' on output will always match,
		// and the pair will be one of:
		//
		//   (double, double)
		//   (float, float)
		//   (ulong, ulong)
		//   (long, long)
		//   (uint, uint)
		//   (int, int)
		//
		static void DoConstantNumericPromotions (EmitContext ec, Binary.Operator oper,
							 ref Constant left, ref Constant right,
							 Location loc)
		{
			
			Type conv_left_as = null;
			Type conv_right_as = null;
			Type lt = left.Type;
			Type rt = right.Type;
	
			
                        if (left is NullLiteral)
                                conv_left_as = rt;
                        if (right is NullLiteral)
                                conv_right_as = lt;
			
			 if (conv_left_as != null)
                                left = ConvertNothingToDefaultConst (ec, conv_left_as, loc);
                        if (conv_right_as != null)
                                right = ConvertNothingToDefaultConst (ec, conv_right_as, loc);
			if (left is DoubleConstant || right is DoubleConstant || 
			    oper == Binary.Operator.Exponentiation || oper == Binary.Operator.Division) {
				//
				// If either side is a double, convert the other to a double
				//
				if (!(left is DoubleConstant))
					left = left.ToDouble (loc);

				if (!(right is DoubleConstant))
					right = right.ToDouble (loc);
				return;
			} else if (left is FloatConstant || right is FloatConstant) {
				//
				// If either side is a float, convert the other to a float
				//
				if (!(left is FloatConstant))
					left = left.ToFloat (loc);

				if (!(right is FloatConstant))
					right = right.ToFloat (loc);
;				return;
			} else if (left is ULongConstant || right is ULongConstant){
				//
				// If either operand is of type ulong, the other operand is
				// converted to type ulong.  or an error ocurrs if the other
				// operand is of type sbyte, short, int or long
				//
#if WRONG
				Constant match, other;
#endif
					
				if (left is ULongConstant){
#if WRONG
					other = right;
					match = left;
#endif
					if (!(right is ULongConstant))
						right = right.ToULong (loc);
				} else {
#if WRONG
					other = left;
					match = right;
#endif
					left = left.ToULong (loc);
				}

#if WRONG
				if (other is SByteConstant || other is ShortConstant ||
				    other is IntConstant || other is LongConstant){
					Binary.Error_OperatorAmbiguous
						(loc, oper, other.Type, match.Type);
					left = null;
					right = null;
				}
#endif
				return;
			} else if (left is LongConstant || right is LongConstant){
				//
				// If either operand is of type long, the other operand is converted
				// to type long.
				//
				if (!(left is LongConstant))
					left = left.ToLong (loc);
				else if (!(right is LongConstant))
					right = right.ToLong (loc);
				return;
			} else if (left is UIntConstant || right is UIntConstant){
				//
				// If either operand is of type uint, and the other
				// operand is of type sbyte, short or int, the operands are
				// converted to type long.
				//
				Constant other;
				if (left is UIntConstant){
					other = right;
				} else {
					other = left;
				}

				// Nothing to do.
				if (other is UIntConstant)
					return;

				if (other is SByteConstant || other is ShortConstant ||
				    other is IntConstant){
					left = left.ToLong (loc);
					right = right.ToLong (loc);
				}

				return;
			} else if (left is EnumConstant || right is EnumConstant){
				//
				// If either operand is an enum constant, the other one must
				// be implicitly convertable to that enum's underlying type.
				//
				EnumConstant match;
				Constant other;
				if (left is EnumConstant){
					other = right;
					match = (EnumConstant) left;
				} else {
					other = left;
					match = (EnumConstant) right;
				}

				bool need_check = (other is EnumConstant) ||
					((oper != Binary.Operator.Addition) &&
					 (oper != Binary.Operator.Subtraction));

				if (need_check &&
				    !Expression.ImplicitConversionExists (ec, match, other.Type)) {
					Expression.Error_CannotConvertImplicit (loc, match.Type, other.Type);
					left = null;
					right = null;
					return;
				}

				if (left is EnumConstant)
					left = ((EnumConstant) left).Child;
				if (right is EnumConstant)
					right = ((EnumConstant) right).Child;
				return;

			} else {
				//
				// Force conversions to int32
				//
				if (!(left is IntConstant))
					left = left.ToInt (loc);
				if (!(right is IntConstant))
					right = right.ToInt (loc);
			}
			return;
		}

		static void Error_CompileTimeOverflow (Location loc)
		{
			Report.Error (220, loc, "The operation overflows at compile time in checked mode");
		}
		
		/// <summary>
		///   Constant expression folder for binary operations.
		///
		///   Returns null if the expression can not be folded.
		/// </summary>
		static public Expression BinaryFold (EmitContext ec, Binary.Operator oper,
						     Constant left, Constant right, Location loc)
		{
			Type lt = left.Type;
			Type rt = right.Type;
			Type result_type = null;
			bool bool_res;
			
			//
			// Enumerator folding
			//
			if (rt == lt && left is EnumConstant)
				result_type = lt;

			//
			// During an enum evaluation, we need to unwrap enumerations
			//
			if (ec.InEnumContext){
				if (left is EnumConstant)
					left = ((EnumConstant) left).Child;
				
				if (right is EnumConstant)
					right = ((EnumConstant) right).Child;
			}

			Type wrap_as;
			Constant result = null;
			switch (oper){
			case Binary.Operator.BitwiseOr:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;
				
				if (left is IntConstant){
					IntConstant v;
					int res = ((IntConstant) left).Value | ((IntConstant) right).Value;
					
					v = new IntConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is UIntConstant){
					UIntConstant v;
					uint res = ((UIntConstant)left).Value | ((UIntConstant)right).Value;
					
					v = new UIntConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is LongConstant){
					LongConstant v;
					long res = ((LongConstant)left).Value | ((LongConstant)right).Value;
					
					v = new LongConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is ULongConstant){
					ULongConstant v;
					ulong res = ((ULongConstant)left).Value |
						((ULongConstant)right).Value;
					
					v = new ULongConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				}
				break;
				
			case Binary.Operator.BitwiseAnd:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;
				
				if (left is IntConstant){
					IntConstant v;
					int res = ((IntConstant) left).Value & ((IntConstant) right).Value;
					
					v = new IntConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is UIntConstant){
					UIntConstant v;
					uint res = ((UIntConstant)left).Value & ((UIntConstant)right).Value;
					
					v = new UIntConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is LongConstant){
					LongConstant v;
					long res = ((LongConstant)left).Value & ((LongConstant)right).Value;
					
					v = new LongConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is ULongConstant){
					ULongConstant v;
					ulong res = ((ULongConstant)left).Value &
						((ULongConstant)right).Value;
					
					v = new ULongConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				}
				break;

			case Binary.Operator.ExclusiveOr:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;
				
				if (left is IntConstant){
					IntConstant v;
					int res = ((IntConstant) left).Value ^ ((IntConstant) right).Value;
					
					v = new IntConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is UIntConstant){
					UIntConstant v;
					uint res = ((UIntConstant)left).Value ^ ((UIntConstant)right).Value;
					
					v = new UIntConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is LongConstant){
					LongConstant v;
					long res = ((LongConstant)left).Value ^ ((LongConstant)right).Value;
					
					v = new LongConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				} else if (left is ULongConstant){
					ULongConstant v;
					ulong res = ((ULongConstant)left).Value ^
						((ULongConstant)right).Value;
					
					v = new ULongConstant (res);
					if (result_type == null)
						return v;
					else
						return new EnumConstant (v, result_type);
				}
				break;

			case Binary.Operator.Addition:
				bool left_is_string = left is StringConstant;
				bool right_is_string = right is StringConstant;

				//
				// If both sides are strings, then concatenate, if
				// one is a string, and the other is not, then defer
				// to runtime concatenation
				//
				wrap_as = null;
				if (left_is_string || right_is_string){
					if (left_is_string && right_is_string)
						return new StringConstant (
							((StringConstant) left).Value +
							((StringConstant) right).Value);
					
					return null;
				}

				//
				// handle "E operator + (E x, U y)"
				// handle "E operator + (Y y, E x)"
				//
				// note that E operator + (E x, E y) is invalid
				//
				if (left is EnumConstant){
					if (right is EnumConstant){
						return null;
					}
					if (((EnumConstant) left).Child.Type != right.Type)
						return null;

					wrap_as = left.Type;
				} else if (right is EnumConstant){
					if (((EnumConstant) right).Child.Type != left.Type)
						return null;
					wrap_as = right.Type;
				}

				result = null;
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				try {
					if (left is DoubleConstant){
						double res;
						
						if (ec.ConstantCheckState)
							res = checked (((DoubleConstant) left).Value +
								       ((DoubleConstant) right).Value);
						else
							res = unchecked (((DoubleConstant) left).Value +
									 ((DoubleConstant) right).Value);
						
						result = new DoubleConstant (res);
					} else if (left is FloatConstant){
						float res;
						
						if (ec.ConstantCheckState)
							res = checked (((FloatConstant) left).Value +
								       ((FloatConstant) right).Value);
						else
							res = unchecked (((FloatConstant) left).Value +
									 ((FloatConstant) right).Value);
						
						result = new FloatConstant (res);
					} else if (left is ULongConstant){
						ulong res;
						
						if (ec.ConstantCheckState)
							res = checked (((ULongConstant) left).Value +
								       ((ULongConstant) right).Value);
						else
							res = unchecked (((ULongConstant) left).Value +
									 ((ULongConstant) right).Value);

						result = new ULongConstant (res);
					} else if (left is LongConstant){
						long res;
						
						if (ec.ConstantCheckState)
							res = checked (((LongConstant) left).Value +
								       ((LongConstant) right).Value);
						else
							res = unchecked (((LongConstant) left).Value +
									 ((LongConstant) right).Value);
						
						result = new LongConstant (res);
					} else if (left is UIntConstant){
						uint res;
						
						if (ec.ConstantCheckState)
							res = checked (((UIntConstant) left).Value +
								       ((UIntConstant) right).Value);
						else
							res = unchecked (((UIntConstant) left).Value +
									 ((UIntConstant) right).Value);
						
						result = new UIntConstant (res);
					} else if (left is IntConstant){
						int res;

						if (ec.ConstantCheckState)
							res = checked (((IntConstant) left).Value +
								       ((IntConstant) right).Value);
						else
							res = unchecked (((IntConstant) left).Value +
									 ((IntConstant) right).Value);

						result = new IntConstant (res);
					} else {
						throw new Exception ( "Unexepected input: " + left);
					}
				} catch (OverflowException){
					Error_CompileTimeOverflow (loc);
				}

				if (wrap_as != null)
					return new EnumConstant (result, wrap_as);
				else
					return result;

			case Binary.Operator.Subtraction:
				//
				// handle "E operator - (E x, U y)"
				// handle "E operator - (Y y, E x)"
				// handle "U operator - (E x, E y)"
				//
				wrap_as = null;
				if (left is EnumConstant){
					if (right is EnumConstant){
						if (left.Type == right.Type)
							wrap_as = TypeManager.EnumToUnderlying (left.Type);
						else
							return null;
					}
					if (((EnumConstant) left).Child.Type != right.Type)
						return null;

					wrap_as = left.Type;
				} else if (right is EnumConstant){
					if (((EnumConstant) right).Child.Type != left.Type)
						return null;
					wrap_as = right.Type;
				}

				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				try {
					if (left is DoubleConstant){
						double res;
						
						if (ec.ConstantCheckState)
							res = checked (((DoubleConstant) left).Value -
								       ((DoubleConstant) right).Value);
						else
							res = unchecked (((DoubleConstant) left).Value -
									 ((DoubleConstant) right).Value);
						
						result = new DoubleConstant (res);
					} else if (left is FloatConstant){
						float res;
						
						if (ec.ConstantCheckState)
							res = checked (((FloatConstant) left).Value -
								       ((FloatConstant) right).Value);
						else
							res = unchecked (((FloatConstant) left).Value -
									 ((FloatConstant) right).Value);
						
						result = new FloatConstant (res);
					} else if (left is ULongConstant){
						ulong res;
						
						if (ec.ConstantCheckState)
							res = checked (((ULongConstant) left).Value -
								       ((ULongConstant) right).Value);
						else
							res = unchecked (((ULongConstant) left).Value -
									 ((ULongConstant) right).Value);
						
						result = new ULongConstant (res);
					} else if (left is LongConstant){
						long res;
						
						if (ec.ConstantCheckState)
							res = checked (((LongConstant) left).Value -
								       ((LongConstant) right).Value);
						else
							res = unchecked (((LongConstant) left).Value -
									 ((LongConstant) right).Value);
						
						result = new LongConstant (res);
					} else if (left is UIntConstant){
						uint res;
						
						if (ec.ConstantCheckState)
							res = checked (((UIntConstant) left).Value -
								       ((UIntConstant) right).Value);
						else
							res = unchecked (((UIntConstant) left).Value -
									 ((UIntConstant) right).Value);
						
						result = new UIntConstant (res);
					} else if (left is IntConstant){
						int res;

						if (ec.ConstantCheckState)
							res = checked (((IntConstant) left).Value -
								       ((IntConstant) right).Value);
						else
							res = unchecked (((IntConstant) left).Value -
									 ((IntConstant) right).Value);

						result = new IntConstant (res);
					} else {
						throw new Exception ( "Unexepected input: " + left);
					}
				} catch (OverflowException){
					Error_CompileTimeOverflow (loc);
				}
				if (wrap_as != null)
					return new EnumConstant (result, wrap_as);
				else
					return result;
				
			case Binary.Operator.Multiply:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				try {
					if (left is DoubleConstant){
						double res;
						
						if (ec.ConstantCheckState)
							res = checked (((DoubleConstant) left).Value *
								       ((DoubleConstant) right).Value);
						else
							res = unchecked (((DoubleConstant) left).Value *
									 ((DoubleConstant) right).Value);
						
						return new DoubleConstant (res);
					} else if (left is FloatConstant){
						float res;
						
						if (ec.ConstantCheckState)
							res = checked (((FloatConstant) left).Value *
								       ((FloatConstant) right).Value);
						else
							res = unchecked (((FloatConstant) left).Value *
									 ((FloatConstant) right).Value);
						
						return new FloatConstant (res);
					} else if (left is ULongConstant){
						ulong res;
						
						if (ec.ConstantCheckState)
							res = checked (((ULongConstant) left).Value *
								       ((ULongConstant) right).Value);
						else
							res = unchecked (((ULongConstant) left).Value *
									 ((ULongConstant) right).Value);
						
						return new ULongConstant (res);
					} else if (left is LongConstant){
						long res;
						
						if (ec.ConstantCheckState)
							res = checked (((LongConstant) left).Value *
								       ((LongConstant) right).Value);
						else
							res = unchecked (((LongConstant) left).Value *
									 ((LongConstant) right).Value);
						
						return new LongConstant (res);
					} else if (left is UIntConstant){
						uint res;
						
						if (ec.ConstantCheckState)
							res = checked (((UIntConstant) left).Value *
								       ((UIntConstant) right).Value);
						else
							res = unchecked (((UIntConstant) left).Value *
									 ((UIntConstant) right).Value);
						
						return new UIntConstant (res);
					} else if (left is IntConstant){
						int res;

						if (ec.ConstantCheckState)
							res = checked (((IntConstant) left).Value *
								       ((IntConstant) right).Value);
						else
							res = unchecked (((IntConstant) left).Value *
									 ((IntConstant) right).Value);

						return new IntConstant (res);
					} else {
						throw new Exception ( "Unexepected input: " + left);
					}
				} catch (OverflowException){
					Error_CompileTimeOverflow (loc);
				}
				break;

			case Binary.Operator.IntDivision:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				try {
					if (Mono.MonoBASIC.Parser.OptionStrict && (!(left is LongConstant) && !(left is IntConstant)) )
					{
						Expression.Error_CannotConvertTypeStrict (left.Type, Type.GetType("System.Int64"), loc);
						
						return null;
					}
					
					if (left is DoubleConstant) {
						long left_val, right_val, res;
						left_val = (long) ((DoubleConstant) left).Value;
						right_val = (long) ((DoubleConstant) right).Value;
						
						if (ec.ConstantCheckState)
							res = checked (left_val / right_val);
						else
							res = unchecked (left_val / right_val);
						
						return new LongConstant (res);
					} else if (left is FloatConstant) {
						long left_val, right_val, res;
						left_val = (long) ((FloatConstant) left).Value;
						right_val = (long) ((FloatConstant) right).Value;
						
						if (ec.ConstantCheckState)
							res = checked (left_val / right_val);
						else
							res = unchecked (left_val / right_val);
						
						return new LongConstant (res);
					} else if (left is DecimalConstant) {
						long left_val, right_val, res;
						left_val = (long) ((DecimalConstant) left).Value;
						right_val = (long) ((DecimalConstant) right).Value;
						
						if (ec.ConstantCheckState)
							res = checked (left_val / right_val);
						else
							res = unchecked (left_val / right_val);
						
						return new LongConstant (res);
					} else if (left is LongConstant) {
						long res;
						
						if (ec.ConstantCheckState)
							res = checked (((LongConstant) left).Value /
								       ((LongConstant) right).Value);
						else
							res = unchecked (((LongConstant) left).Value /
									 ((LongConstant) right).Value);
						
						return new LongConstant (res);
					} else {
						int res;
						
						if (ec.ConstantCheckState)
							res = checked (((IntConstant) left).Value /
								       ((IntConstant) right).Value);
						else
							res = unchecked (((IntConstant) left).Value /
									 ((IntConstant) right).Value);
						
						return new IntConstant (res);
					}
				} catch (OverflowException){
					Error_CompileTimeOverflow (loc);

				} catch (DivideByZeroException) {
					Report.Error (30542, loc, "Division by constant zero");
				}
				
				break;
			case Binary.Operator.Division:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				try {
					double res;
					
					if (ec.ConstantCheckState)
						res = checked (((DoubleConstant) left).Value /
							       ((DoubleConstant) right).Value);
					else
						res = unchecked (((DoubleConstant) left).Value /
								 ((DoubleConstant) right).Value);
					
					return new DoubleConstant (res);
				} catch (OverflowException){
					Error_CompileTimeOverflow (loc);

				} catch (DivideByZeroException) {
					Report.Error (30542, loc, "Division by constant zero");
				}
				
				break;
				
			case Binary.Operator.Modulus:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				try {
					if (left is DoubleConstant){
						double res;
						
						if (ec.ConstantCheckState)
							res = checked (((DoubleConstant) left).Value %
								       ((DoubleConstant) right).Value);
						else
							res = unchecked (((DoubleConstant) left).Value %
									 ((DoubleConstant) right).Value);
						
						return new DoubleConstant (res);
					} else if (left is FloatConstant){
						float res;
						
						if (ec.ConstantCheckState)
							res = checked (((FloatConstant) left).Value %
								       ((FloatConstant) right).Value);
						else
							res = unchecked (((FloatConstant) left).Value %
									 ((FloatConstant) right).Value);
						
						return new FloatConstant (res);
					} else if (left is ULongConstant){
						ulong res;
						
						if (ec.ConstantCheckState)
							res = checked (((ULongConstant) left).Value %
								       ((ULongConstant) right).Value);
						else
							res = unchecked (((ULongConstant) left).Value %
									 ((ULongConstant) right).Value);
						
						return new ULongConstant (res);
					} else if (left is LongConstant){
						long res;
						
						if (ec.ConstantCheckState)
							res = checked (((LongConstant) left).Value %
								       ((LongConstant) right).Value);
						else
							res = unchecked (((LongConstant) left).Value %
									 ((LongConstant) right).Value);
						
						return new LongConstant (res);
					} else if (left is UIntConstant){
						uint res;
						
						if (ec.ConstantCheckState)
							res = checked (((UIntConstant) left).Value %
								       ((UIntConstant) right).Value);
						else
							res = unchecked (((UIntConstant) left).Value %
									 ((UIntConstant) right).Value);
						
						return new UIntConstant (res);
					} else if (left is IntConstant){
						int res;

						if (ec.ConstantCheckState)
							res = checked (((IntConstant) left).Value %
								       ((IntConstant) right).Value);
						else
							res = unchecked (((IntConstant) left).Value %
									 ((IntConstant) right).Value);

						return new IntConstant (res);
					} else {
						throw new Exception ( "Unexepected input: " + left);
					}
				} catch (OverflowException){
					Error_CompileTimeOverflow (loc);

				} catch (DivideByZeroException) {
                                        Report.Error (30542, loc, "Division by constant zero");
                                }
				break;

			case Binary.Operator.Exponentiation:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				double powVal = System.Math.Pow(((DoubleConstant) left).Value, 
								((DoubleConstant) right).Value);

				return new DoubleConstant(powVal);

				//
				// There is no overflow checking on left shift
				//
			case Binary.Operator.LeftShift:
				IntConstant ic = right.ToInt (loc);
				if (ic == null){
					Binary.Error_OperatorCannotBeApplied (loc, "<<", lt, rt);
					return null;
				}
				int lshift_val = ic.Value;

				IntConstant lic;
				if ((lic = left.ConvertToInt ()) != null)
					return new IntConstant (lic.Value << lshift_val);

				UIntConstant luic;
				if ((luic = left.ConvertToUInt ()) != null)
					return new UIntConstant (luic.Value << lshift_val);

				LongConstant llc;
				if ((llc = left.ConvertToLong ()) != null)
					return new LongConstant (llc.Value << lshift_val);

				ULongConstant lulc;
				if ((lulc = left.ConvertToULong ()) != null)
					return new ULongConstant (lulc.Value << lshift_val);

				Binary.Error_OperatorCannotBeApplied (loc, "<<", lt, rt);
				break;

				//
				// There is no overflow checking on right shift
				//
			case Binary.Operator.RightShift:
				IntConstant sic = right.ToInt (loc);
				if (sic == null){
					Binary.Error_OperatorCannotBeApplied (loc, ">>", lt, rt);
					return null;
				}
				int rshift_val = sic.Value;

				IntConstant ric;
				if ((ric = left.ConvertToInt ()) != null)
					return new IntConstant (ric.Value >> rshift_val);

				UIntConstant ruic;
				if ((ruic = left.ConvertToUInt ()) != null)
					return new UIntConstant (ruic.Value >> rshift_val);

				LongConstant rlc;
				if ((rlc = left.ConvertToLong ()) != null)
					return new LongConstant (rlc.Value >> rshift_val);

				ULongConstant rulc;
				if ((rulc = left.ConvertToULong ()) != null)
					return new ULongConstant (rulc.Value >> rshift_val);

				Binary.Error_OperatorCannotBeApplied (loc, ">>", lt, rt);
				break;

			case Binary.Operator.LogicalAnd:
				if (left is BoolConstant && right is BoolConstant){
					return new BoolConstant (
						((BoolConstant) left).Value &&
						((BoolConstant) right).Value);
				}
				break;

			case Binary.Operator.LogicalOr:
				if (left is BoolConstant && right is BoolConstant){
					return new BoolConstant (
						((BoolConstant) left).Value ||
						((BoolConstant) right).Value);
				}
				break;
				
			case Binary.Operator.Equality:
				if (left is BoolConstant && right is BoolConstant){
					return new BoolConstant (
						((BoolConstant) left).Value ==
						((BoolConstant) right).Value);
				
				}
				if (left is StringConstant && right is StringConstant){
					return new BoolConstant (
						((StringConstant) left).Value ==
						((StringConstant) right).Value);
					
				}
				
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				bool_res = false;
				if (left is DoubleConstant)
					bool_res = ((DoubleConstant) left).Value ==
						((DoubleConstant) right).Value;
				else if (left is FloatConstant)
					bool_res = ((FloatConstant) left).Value ==
						((FloatConstant) right).Value;
				else if (left is ULongConstant)
					bool_res = ((ULongConstant) left).Value ==
						((ULongConstant) right).Value;
				else if (left is LongConstant)
					bool_res = ((LongConstant) left).Value ==
						((LongConstant) right).Value;
				else if (left is UIntConstant)
					bool_res = ((UIntConstant) left).Value ==
						((UIntConstant) right).Value;
				else if (left is IntConstant)
					bool_res = ((IntConstant) left).Value ==
						((IntConstant) right).Value;
				else
					return null;

				return new BoolConstant (bool_res);

			case Binary.Operator.Inequality:
				if (left is BoolConstant && right is BoolConstant){
					return new BoolConstant (
						((BoolConstant) left).Value !=
						((BoolConstant) right).Value);
				}
				if (left is StringConstant && right is StringConstant){
					return new BoolConstant (
						((StringConstant) left).Value !=
						((StringConstant) right).Value);
					
				}
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				bool_res = false;
				if (left is DoubleConstant)
					bool_res = ((DoubleConstant) left).Value !=
						((DoubleConstant) right).Value;
				else if (left is FloatConstant)
					bool_res = ((FloatConstant) left).Value !=
						((FloatConstant) right).Value;
				else if (left is ULongConstant)
					bool_res = ((ULongConstant) left).Value !=
						((ULongConstant) right).Value;
				else if (left is LongConstant)
					bool_res = ((LongConstant) left).Value !=
						((LongConstant) right).Value;
				else if (left is UIntConstant)
					bool_res = ((UIntConstant) left).Value !=
						((UIntConstant) right).Value;
				else if (left is IntConstant)
					bool_res = ((IntConstant) left).Value !=
						((IntConstant) right).Value;
				else
					return null;

				return new BoolConstant (bool_res);

			case Binary.Operator.LessThan:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				bool_res = false;
				if (left is DoubleConstant)
					bool_res = ((DoubleConstant) left).Value <
						((DoubleConstant) right).Value;
				else if (left is FloatConstant)
					bool_res = ((FloatConstant) left).Value <
						((FloatConstant) right).Value;
				else if (left is ULongConstant)
					bool_res = ((ULongConstant) left).Value <
						((ULongConstant) right).Value;
				else if (left is LongConstant)
					bool_res = ((LongConstant) left).Value <
						((LongConstant) right).Value;
				else if (left is UIntConstant)
					bool_res = ((UIntConstant) left).Value <
						((UIntConstant) right).Value;
				else if (left is IntConstant)
					bool_res = ((IntConstant) left).Value <
						((IntConstant) right).Value;
				else
					return null;

				return new BoolConstant (bool_res);
				
			case Binary.Operator.GreaterThan:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				bool_res = false;
				if (left is DoubleConstant)
					bool_res = ((DoubleConstant) left).Value >
						((DoubleConstant) right).Value;
				else if (left is FloatConstant)
					bool_res = ((FloatConstant) left).Value >
						((FloatConstant) right).Value;
				else if (left is ULongConstant)
					bool_res = ((ULongConstant) left).Value >
						((ULongConstant) right).Value;
				else if (left is LongConstant)
					bool_res = ((LongConstant) left).Value >
						((LongConstant) right).Value;
				else if (left is UIntConstant)
					bool_res = ((UIntConstant) left).Value >
						((UIntConstant) right).Value;
				else if (left is IntConstant)
					bool_res = ((IntConstant) left).Value >
						((IntConstant) right).Value;
				else
					return null;

				return new BoolConstant (bool_res);

			case Binary.Operator.GreaterThanOrEqual:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				bool_res = false;
				if (left is DoubleConstant)
					bool_res = ((DoubleConstant) left).Value >=
						((DoubleConstant) right).Value;
				else if (left is FloatConstant)
					bool_res = ((FloatConstant) left).Value >=
						((FloatConstant) right).Value;
				else if (left is ULongConstant)
					bool_res = ((ULongConstant) left).Value >=
						((ULongConstant) right).Value;
				else if (left is LongConstant)
					bool_res = ((LongConstant) left).Value >=
						((LongConstant) right).Value;
				else if (left is UIntConstant)
					bool_res = ((UIntConstant) left).Value >=
						((UIntConstant) right).Value;
				else if (left is IntConstant)
					bool_res = ((IntConstant) left).Value >=
						((IntConstant) right).Value;
				else
					return null;

				return new BoolConstant (bool_res);

			case Binary.Operator.LessThanOrEqual:
				DoConstantNumericPromotions (ec, oper, ref left, ref right, loc);
				if (left == null || right == null)
					return null;

				bool_res = false;
				if (left is DoubleConstant)
					bool_res = ((DoubleConstant) left).Value <=
						((DoubleConstant) right).Value;
				else if (left is FloatConstant)
					bool_res = ((FloatConstant) left).Value <=
						((FloatConstant) right).Value;
				else if (left is ULongConstant)
					bool_res = ((ULongConstant) left).Value <=
						((ULongConstant) right).Value;
				else if (left is LongConstant)
					bool_res = ((LongConstant) left).Value <=
						((LongConstant) right).Value;
				else if (left is UIntConstant)
					bool_res = ((UIntConstant) left).Value <=
						((UIntConstant) right).Value;
				else if (left is IntConstant)
					bool_res = ((IntConstant) left).Value <=
						((IntConstant) right).Value;
				else
					return null;

				return new BoolConstant (bool_res);
			}
					
			return null;
		}
	}
}