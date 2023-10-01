import pyodbc
import time

conn_str = (
    r"Driver={ODBC Driver 17 for SQL Server};"
    r"Server=localhost;"
    r"Database=master;"
    r"UID=sa;"
    r"PWD=MyPass@word;" 
)
conn = pyodbc.connect(conn_str, timeout=300)
cursor = conn.cursor()

sql_files = [
    "/sql/Role.sql",
    "/sql/Person.sql",
    "/sql/Borrower.sql",
    "/sql/Loan.sql",
    "/sql/PersonToLoan.sql",
    "/sql/Transact.sql"
]

for sql_file in sql_files:
    with open(sql_file, 'r') as f:
        sql_command = f.read()
        cursor.execute(sql_command)
        cursor.commit()

cursor.close()
conn.close()
