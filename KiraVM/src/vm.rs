use std::io::{Cursor, Read};

use internal;
use literal::Literal;
use read::{read_i32, read_i64, read_string, read_literal};

const INSTRUCTION_PUSH: i32 = 0x01;
const INSTRUCTION_POP: i32 = 0x02;

const INSTRUCTION_SET: i32 = 0x03;

const INSTRUCTION_CALLS: i32 = 0x10;
const INSTRUCTION_CALLI: i32 = 0x11;
const INSTRUCTION_CALLE: i32 = 0x12;

pub fn run(mut cursor: Cursor<Vec<u8>>) {
	let local_count = read_i32(&mut cursor) as usize;
	let mut locals: Vec<Literal> = Vec::with_capacity(local_count);

	for _ in 0..local_count {
		locals.push(Literal::Integer(0));
	}

	let string_count = read_i32(&mut cursor) as usize;
	let mut strings: Vec<String> = Vec::with_capacity(string_count);

	for _ in 0..string_count {
		strings.push(read_string(&mut cursor));
	}

	let program_length = read_i64(&mut cursor);
	let mut program = vec![0; program_length as usize];
	let mut stack: Vec<Literal> = Vec::new();

	cursor.read_exact(&mut program).expect("Failed to read program");

	let mut cursor = Cursor::new(program);

	while cursor.position() < program_length as u64 {
		let instruction = read_i32(&mut cursor);

		match instruction {
			INSTRUCTION_PUSH => {
				stack.push(read_literal(&mut cursor, &strings));
			},
			INSTRUCTION_POP => {
				stack.pop();
			},

			INSTRUCTION_SET => {
				let index = read_i32(&mut cursor) as usize;
				let value = read_literal(&mut cursor, &strings);

				locals[index] = value;
			},

			INSTRUCTION_CALLS => {} // TODO: Implement
			INSTRUCTION_CALLI => {
				internal::execute(read_i32(&mut cursor), &mut stack);
			},
			INSTRUCTION_CALLE => {} // TODO: Implement

			_ => { throw_error!("Got unknown instruction: {}", instruction); }
		};
	}
}
