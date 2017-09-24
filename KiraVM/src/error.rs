#[macro_export]
macro_rules! throw_error {
    ($msg:expr) => {{
    	use std::process::exit;

		println!($msg);
    	exit(1);
    }};
    ($msg:expr, $($arg:tt)*) => {{
    	use std::process::exit;

    	println!($msg, $($arg)*);
    	exit(1);
    }}
}
