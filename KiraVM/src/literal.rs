use std::fmt::{Display, Formatter, Result};

pub const LITERAL_INTEGER: i8 = 0x0;
pub const LITERAL_FLOAT: i8 = 0x1;
pub const LITERAL_STRING: i8 = 0x2;

pub enum Literal {
	Integer(i32),
	Float(f32),
	String(String)
}

impl Default for Literal {

	fn default() -> Literal {
		Literal::Integer(0)
	}
}

impl Display for Literal {

	fn fmt(&self, f: &mut Formatter) -> Result {
		match *self {
			Literal::Integer(i) => write!(f, "{}", i),
			Literal::Float(fl) => write!(f, "{}", fl),
			Literal::String(ref s) => write!(f, "{}", s)	
		}
	}
}
