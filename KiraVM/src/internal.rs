use literal::Literal;

const INTERNAL_PRINT: i32 = 0xF0;
const INTERNAL_PRINTLN: i32 = 0xF1;

pub fn execute(internal: i32, stack: &mut Vec<Literal>) {
	match internal {
		INTERNAL_PRINT => {
			let value = stack.pop().expect("Tried to call internal \"print\" with no input value.");

			print!("{}", value);
		},

		INTERNAL_PRINTLN => {
			let value = stack.pop().expect("Tried to call internal \"println\" with no input value.");

			println!("{}", value);
		},

		_ => {} // TODO: Throw error.
	}
}
