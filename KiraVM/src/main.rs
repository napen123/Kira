extern crate byteorder;

#[macro_use]
mod error;

mod vm;
mod read;
mod literal;
mod internal;

use std::env;
use std::fs::OpenOptions;
use std::io::{Cursor, Read};

fn main() {
	let args: Vec<String> = env::args().collect();

	if args.len() != 2 {
		println!("Usage : kiravm [input file]");

		return;
	}

	let mut file = OpenOptions::new().read(true).open(&args[1]).expect("Could not open input file");
	let mut header = [0u8; 4];

	file.read_exact(&mut header).expect("Could not read file header");

	if header[0] != 'K' as u8 ||
	   header[1] != 'I' as u8 ||
	   header[2] != 'R' as u8 ||
	   header[3] != 'A' as u8 {
	   	throw_error!("Invalid file header.");
	}

	let mut contents = Vec::new();
	file.read_to_end(&mut contents).unwrap();
	let cursor = Cursor::new(contents);

	vm::run(cursor);
}
