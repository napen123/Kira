use std::process;
use std::io::{Cursor, Read};

use byteorder::{ReadBytesExt, LittleEndian};

use literal::{Literal, LITERAL_INTEGER, LITERAL_FLOAT, LITERAL_STRING};

pub fn read_i8(cursor: &mut Cursor<Vec<u8>>) -> i8 {
	match cursor.read_i8() {
		Ok(i) => i,
		Err(err) => {
			eprintln!("Error : Failed to read i8 : {}", err);
			process::exit(1);
		}
	}
}

pub fn read_i32(cursor: &mut Cursor<Vec<u8>>) -> i32 {
	match cursor.read_i32::<LittleEndian>() {
		Ok(i) => i,
		Err(err) => {
			eprintln!("Error : Failed to read i32 : {}", err);
			process::exit(1);
		}
	}
}

pub fn read_i64(cursor: &mut Cursor<Vec<u8>>) -> i64 {
	match cursor.read_i64::<LittleEndian>() {
		Ok(i) => i,
		Err(err) => {
			eprintln!("Error : Failed to read i64 : {}", err);
			process::exit(1);
		}
	}
}

pub fn read_f32(cursor: &mut Cursor<Vec<u8>>) -> f32 {
	match cursor.read_f32::<LittleEndian>() {
		Ok(i) => i,
		Err(err) => {
			eprintln!("Error : Failed to read f32 : {}", err);
			process::exit(1);
		}
	}
}

pub fn read_string(cursor: &mut Cursor<Vec<u8>>) -> String {
	let length = read_i32(cursor) as usize;
	let mut utf8 = vec![0; length];

	match cursor.read_exact(&mut utf8) {
		Ok(_) => {},
		Err(err) => {
			eprintln!("Error : Failed to read string : {}", err);
			process::exit(1);
		}
	}

	match String::from_utf8(utf8) {
		Ok(s) => s,
		Err(err) => {
			eprintln!("Error : Failed to read string : {}", err);
			process::exit(1);
		}
	}
}

pub fn read_literal(cursor: &mut Cursor<Vec<u8>>, strings: &[String]) -> Literal {
	match read_i8(cursor) {
		LITERAL_INTEGER => Literal::Integer(read_i32(cursor)),
		LITERAL_FLOAT => Literal::Float(read_f32(cursor)),
		LITERAL_STRING => {
			let i = read_i32(cursor) as usize;

			Literal::String(strings[i].clone())
		},
		_ => {
			eprintln!("Error : Failed to read literal value.");
			process::exit(1);
		}
	}
}
